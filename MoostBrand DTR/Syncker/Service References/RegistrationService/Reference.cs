﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Syncker.RegistrationService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="RegistrationService.RegistrationServiceSoap")]
    public interface RegistrationServiceSoap {
        
        // CODEGEN: Generating message contract since element name _log from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Synchronize", ReplyAction="*")]
        Syncker.RegistrationService.SynchronizeResponse Synchronize(Syncker.RegistrationService.SynchronizeRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Synchronize", ReplyAction="*")]
        System.Threading.Tasks.Task<Syncker.RegistrationService.SynchronizeResponse> SynchronizeAsync(Syncker.RegistrationService.SynchronizeRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SynchronizeRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Synchronize", Namespace="http://tempuri.org/", Order=0)]
        public Syncker.RegistrationService.SynchronizeRequestBody Body;
        
        public SynchronizeRequest() {
        }
        
        public SynchronizeRequest(Syncker.RegistrationService.SynchronizeRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class SynchronizeRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string _log;
        
        public SynchronizeRequestBody() {
        }
        
        public SynchronizeRequestBody(string _log) {
            this._log = _log;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SynchronizeResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="SynchronizeResponse", Namespace="http://tempuri.org/", Order=0)]
        public Syncker.RegistrationService.SynchronizeResponseBody Body;
        
        public SynchronizeResponse() {
        }
        
        public SynchronizeResponse(Syncker.RegistrationService.SynchronizeResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class SynchronizeResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public int SynchronizeResult;
        
        public SynchronizeResponseBody() {
        }
        
        public SynchronizeResponseBody(int SynchronizeResult) {
            this.SynchronizeResult = SynchronizeResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface RegistrationServiceSoapChannel : Syncker.RegistrationService.RegistrationServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class RegistrationServiceSoapClient : System.ServiceModel.ClientBase<Syncker.RegistrationService.RegistrationServiceSoap>, Syncker.RegistrationService.RegistrationServiceSoap {
        
        public RegistrationServiceSoapClient() {
        }
        
        public RegistrationServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public RegistrationServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RegistrationServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RegistrationServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Syncker.RegistrationService.SynchronizeResponse Syncker.RegistrationService.RegistrationServiceSoap.Synchronize(Syncker.RegistrationService.SynchronizeRequest request) {
            return base.Channel.Synchronize(request);
        }
        
        public int Synchronize(string _log) {
            Syncker.RegistrationService.SynchronizeRequest inValue = new Syncker.RegistrationService.SynchronizeRequest();
            inValue.Body = new Syncker.RegistrationService.SynchronizeRequestBody();
            inValue.Body._log = _log;
            Syncker.RegistrationService.SynchronizeResponse retVal = ((Syncker.RegistrationService.RegistrationServiceSoap)(this)).Synchronize(inValue);
            return retVal.Body.SynchronizeResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Syncker.RegistrationService.SynchronizeResponse> Syncker.RegistrationService.RegistrationServiceSoap.SynchronizeAsync(Syncker.RegistrationService.SynchronizeRequest request) {
            return base.Channel.SynchronizeAsync(request);
        }
        
        public System.Threading.Tasks.Task<Syncker.RegistrationService.SynchronizeResponse> SynchronizeAsync(string _log) {
            Syncker.RegistrationService.SynchronizeRequest inValue = new Syncker.RegistrationService.SynchronizeRequest();
            inValue.Body = new Syncker.RegistrationService.SynchronizeRequestBody();
            inValue.Body._log = _log;
            return ((Syncker.RegistrationService.RegistrationServiceSoap)(this)).SynchronizeAsync(inValue);
        }
    }
}
