using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Lib.Helpers;
using Microsoft.CSharp;
using Microsoft.Extensions.Logging;

namespace Lib.Services
{
    public class SoapService : ISoapService
    {
        private readonly IGenericMapper _mapper;
        private readonly ILogger<SoapService> _logger;
        private readonly SoapServiceCredentials _credentials;
        private readonly ServiceDescription _serviceDescription;
        private readonly Type _serviceType;

        private const string ProtocolName = @"Soap";
        private const string DiscoverName = @"Discover";
        private const string SystemXmlDll = @"System.Xml.dll";
        private const string WebServiceDll = @"System.Web.Services.dll";

        public static ISoapService BuildSoapService(SoapServiceCredentials credentials)
        {
            var mapper = new GenericMapper();
            var loggerFactory = new LoggerFactory();
            var logger = loggerFactory.CreateLogger<SoapService>();
            return new SoapService(credentials, mapper, logger);
        }

        public SoapService(SoapServiceCredentials credentials, IGenericMapper mapper, ILogger<SoapService> logger)
        {
            _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceDescription = BuildServiceDescription();
            _serviceType = BuildServiceType();
        }

        public ICollection<MethodInfo> GetMethodInfos()
        {
            return _serviceType.GetMethods()
                .Where(x => !x.Name.Equals(DiscoverName))
                .Where(mInfo => mInfo.GetCustomAttributes<SoapDocumentMethodAttribute>(true).Any())
                .ToList();
        }

        public object RunMethod(string methodName, params object[] passedParams)
        {
            var methodInfos = GetMethodInfos();
            var method = methodInfos?.SingleOrDefault(x => x.Name.Equals(methodName, StringComparison.InvariantCultureIgnoreCase));
            if (method is null)
            {
                throw new Exception($"Method '{methodName}' does not exist!");
            }

            var methodParams = method.GetParameters().ToArray();
            if (passedParams == null)
            {
                passedParams = new object[] { };
            }

            if (passedParams.Length != methodParams.Length)
            {
                throw new Exception($"No method '{methodName}' with '{passedParams.Length}' parameters!");
            }

            var index = 0;
            var paramsToPass = methodParams.Select(pi => _mapper.Map(passedParams[index++], pi.ParameterType)).ToArray();
            var serviceInstance = Activator.CreateInstance(_serviceType);
            return method.Invoke(serviceInstance, paramsToPass);
        }

        private ServiceDescription BuildServiceDescription()
        {
            var url = _credentials.Url;

            if (!SoapServiceCredentials.IsValidUrl(url))
            {
                throw new Exception($"Url '{url}' is not valid");
            }

            var httpClient = HttpClientFactory.Create();

            if (!string.IsNullOrWhiteSpace(_credentials.Username) && !string.IsNullOrWhiteSpace(_credentials.Password))
            {
                var authInfo = $"{_credentials.Username}:{_credentials.Password}";
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authInfo);
            }

            using (var response = httpClient.GetStreamAsync(url).GetAwaiter().GetResult())
            {
                if (response is null)
                {
                    throw new Exception($"Failed to read url '{url}'");
                }

                var description = ServiceDescription.Read(response);
                if (description.Services.Count == 0)
                {
                    throw new Exception($"Service name for url '{url}' is not found");
                }

                return description;
            }
        }

        private Type BuildServiceType()
        {
            var serviceName = _serviceDescription.Services[0].Name;
            var sdImport = new ServiceDescriptionImporter();
            sdImport.AddServiceDescription(_serviceDescription, string.Empty, string.Empty);
            sdImport.ProtocolName = ProtocolName;
            sdImport.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties;
            var codeNs = new CodeNamespace();
            var codeUn = new CodeCompileUnit();
            codeUn.Namespaces.Add(codeNs);
            var warnings = sdImport.Import(codeNs, codeUn);
            if (warnings == 0)
            {
                _logger.LogWarning("Warnings found {@warnings}", warnings);
            }

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
                prov.GenerateCodeFromNamespace(codeNs, stringWriter, new CodeGeneratorOptions());
                var results = prov.CompileAssemblyFromDom(param, codeUn);
                var assembly = results.CompiledAssembly;
                return assembly.GetType(serviceName);
            }
        }
    }
}
