using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Microsoft.CSharp;

namespace Lib.Helpers
{
    public class ServiceInspector : IServiceInspector
    {
        private string _sdName;
        private Stream _response;
        private Type _serviceType;
        private CodeNamespace _codeNs;
        private CodeCompileUnit _codeUn;
        private readonly IGenericMapper _mapper;
        private const string QueryUri = @"?WSDL";
        private const string ProtocolName = @"Soap";
        private const string DiscoverName = @"Discover";
        private const string SystemXmlDll = @"System.Xml.dll";
        private const string WebServiceDll = @"System.Web.Services.dll";
        public ICollection<MethodInfo> MethodInfos { get; private set; }

        protected ServiceInspector(IGenericMapper mapper)
        {
            _mapper = mapper ?? new GenericMapper();
        }

        public ServiceInspector(string uri, IGenericMapper mapper = null) : this(mapper)
        {
            if (!IsValidUri(uri)) throw new ArgumentException($"Service Uri {uri} is not valid!");
            if (!IsServiceUp(uri)) throw new ArgumentException($"Service {uri} is not responding!");
            BuildServiceDescription();
            BuildServiceAssembly();
            FillServiceMethods();
        }

        public bool HasMethod(string methodName, out MethodInfo method)
        {
            method = MethodInfos?.SingleOrDefault(x => x.Name.Equals(methodName, StringComparison.InvariantCultureIgnoreCase));
            return method != null;
        }

        public ICollection<ParameterInfo> GetMethodParameters(string methodName)
        {
            return HasMethod(methodName, out var method) ? method.GetParameters() : new ParameterInfo[] { };
        }

        public object RunMethod(string methodName, params object[] passedParams)
        {
            if (!HasMethod(methodName, out var method))
            {
                Debug.WriteLine($"Method {methodName} does not exist!");
                return null;
            }

            var methodParams = method.GetParameters().ToArray();
            if (passedParams == null) passedParams = new object[] { };

            if (passedParams.Length != methodParams.Length)
            {
                throw new ArgumentException($"No method {methodName} with {passedParams.Length} parameters!");
            }

            var index = 0;
            var paramsToPass = methodParams.Select(pi => _mapper.Map(passedParams[index++], pi.ParameterType)).ToArray();
            var serviceInstance = Activator.CreateInstance(_serviceType);
            return method.Invoke(serviceInstance, paramsToPass);
        }

        private static bool IsValidUri(string uri)
        {
            try
            {
                var queryUri = new UriBuilder(uri).Query.ToUpper();
                return queryUri.Equals(QueryUri);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private bool IsServiceUp(string uri)
        {
            try
            {
                var request = WebRequest.Create(uri);
                _response = request.GetResponse().GetResponseStream();
                return _response != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _response = null;
                return false;
            }
        }

        private void BuildServiceDescription()
        {
            var webService = ServiceDescription.Read(_response);
            _sdName = webService.Services[0].Name;
            var sdImport = new ServiceDescriptionImporter();
            sdImport.AddServiceDescription(webService, string.Empty, string.Empty);
            sdImport.ProtocolName = ProtocolName;
            sdImport.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties;
            _codeNs = new CodeNamespace();
            _codeUn = new CodeCompileUnit();
            _codeUn.Namespaces.Add(_codeNs);
            var warnings = sdImport.Import(_codeNs, _codeUn);
            if (warnings == 0) return;
            Console.WriteLine($"Warnings {warnings}!");
        }

        private void BuildServiceAssembly()
        {
            var assemblyReferences = new[] { SystemXmlDll, WebServiceDll };
            var param = new CompilerParameters(assemblyReferences)
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                TreatWarningsAsErrors = false,
                WarningLevel = 4
            };

            var stringWriter = new StringWriter(CultureInfo.CurrentCulture);
            using (var prov = new CSharpCodeProvider())
            {
                prov.GenerateCodeFromNamespace(_codeNs, stringWriter, new CodeGeneratorOptions());
                var results = prov.CompileAssemblyFromDom(param, _codeUn);
                var assembly = results.CompiledAssembly;
                _serviceType = assembly.GetType(_sdName);
            }
        }

        private void FillServiceMethods()
        {
            MethodInfos = new List<MethodInfo>();
            foreach (var mInfo in _serviceType.GetMethods().Where(x => !x.Name.Equals(DiscoverName)))
            {
                if (mInfo.GetCustomAttributes<SoapDocumentMethodAttribute>(true).Any())
                {
                    MethodInfos.Add(mInfo);
                }
            }
        }
    }
}
