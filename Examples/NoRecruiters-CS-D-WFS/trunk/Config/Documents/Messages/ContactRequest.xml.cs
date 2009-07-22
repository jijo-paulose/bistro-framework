#pragma warning disable 0169, 0649, 0618
// -------------------------------------------864187d2-c0eb-4620-9c81-c789026db391
//
//		Generated document definition classes
//		Workflow Server code generator v.4.0.1.0
//		All modifications to the content of this file will be lost
//		when file will be overwritten by the code generator
//
//		Generated from the Xml schema located at 
//			Config\Documents\Messages\ContactRequest.xml
//			as of 6/22/2009 2:15 
//
// -------------------------------------------------------------------------------
namespace NoRecruiters3 {
    using WorkflowServer.Foundation.Documents;
    using WorkflowServer.Foundation.Documents.MetaInfo;
    
    
    /// WS_documentDependency WorkflowServer.Foundation.Documents.Message=Unknown
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="ContactRequest", IsReference=true)]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(messageNode_Node))]
    [DocumentVersion("b995a0f4deb6a539ca45b51370bdb3531f2492df")]
    public partial class ContactRequest : WorkflowServer.Foundation.Documents.Message {
        
        // Contains names of metanodes. Used at run time by the ElementMetaNode.Build method
        static string[] WS_metanode_names = new string[] {
                "docID",
                "messageNode",
                "ToAddress",
                "CompanyName",
                "CompanyUrl",
                "ContactText"};
        
        // Local metanode index. Populated runtime by the ElmentMetaNode.Build method
        static WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode[] WS_metanodes;
        
        public static string WS_documentSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns:wdfext=""http://schemas.hill30.com/workflow/documents"" xmlns:wdf=""http://schemas.hill30.com/workflow/documents"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element name=""document"">
    <xs:complexType>
      <xs:sequence minOccurs=""1"">
        <xs:element name=""messageNode"">
          <xs:complexType>
            <xs:attribute wdfext:param-name=""activity"" name=""activity"" type=""xs:string"" />
            <xs:attribute wdfext:param-name=""suspended"" name=""suspended"" type=""xs:boolean"" />
            <xs:attribute wdfext:param-name=""posted_at"" name=""postedAt"" type=""xs:dateTime"" />
            <xs:attribute wdfext:param-name=""escalate_at"" name=""escalateAt"" type=""xs:dateTime"" />
          </xs:complexType>
        </xs:element>
      </xs:sequence>
      <xs:attribute name=""docID"" type=""xs:ID"" />
      <xs:attribute name=""ToAddress"" type=""xs:string"" />
      <xs:attribute name=""CompanyName"" type=""xs:string"" />
      <xs:attribute name=""CompanyUrl"" type=""xs:string"" />
      <xs:attribute name=""ContactText"" type=""xs:string"" />
    </xs:complexType>
  </xs:element>
</xs:schema>";
        
        // Private field for the Value value of the the ToAddress document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v2", Order=2002, IsRequired=false)]
        private string v2;
        
        // Private field for the Original value of the the ToAddress document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o2", Order=3002, IsRequired=false)]
        private string o2;
        
        // Private field for the Value value of the the CompanyName document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v3", Order=2003, IsRequired=false)]
        private string v3;
        
        // Private field for the Original value of the the CompanyName document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o3", Order=3003, IsRequired=false)]
        private string o3;
        
        // Private field for the Value value of the the CompanyUrl document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v4", Order=2004, IsRequired=false)]
        private string v4;
        
        // Private field for the Original value of the the CompanyUrl document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o4", Order=3004, IsRequired=false)]
        private string o4;
        
        // Private field for the Value value of the the ContactText document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v5", Order=2005, IsRequired=false)]
        private string v5;
        
        // Private field for the Original value of the the ContactText document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o5", Order=3005, IsRequired=false)]
        private string o5;
        
        public ContactRequest(WorkflowServer.Foundation.Documents.DocumentFactory factory, System.Xml.XmlReader content) : 
                base(factory, content) {
            return;
        }
        
        public ContactRequest(WorkflowServer.Foundation.Documents.DocumentFactory factory) : 
                base(factory) {
            return;
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(FieldType=typeof(NoRecruiters3.ContactRequest.messageNode_Node))]
        public new virtual NoRecruiters3.ContactRequest.messageNode_Node messageNode {
            get {
                return ((NoRecruiters3.ContactRequest.messageNode_Node)(base.messageNode));
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v2", OriginalFieldName="o2", Name="ToAddress")]
        private string @__ToAddress {
            get {
                return v2;
            }
            set {
                if ((v2 != value)) {
                    v2 = value;
                    FieldModified(WS_metanodes[2]);
                }
            }
        }
        
        public virtual string ToAddress {
            get {
                return @__ToAddress;
            }
            set {
                @__ToAddress = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v3", OriginalFieldName="o3", Name="CompanyName")]
        private string @__CompanyName {
            get {
                return v3;
            }
            set {
                if ((v3 != value)) {
                    v3 = value;
                    FieldModified(WS_metanodes[3]);
                }
            }
        }
        
        public virtual string CompanyName {
            get {
                return @__CompanyName;
            }
            set {
                @__CompanyName = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v4", OriginalFieldName="o4", Name="CompanyUrl")]
        private string @__CompanyUrl {
            get {
                return v4;
            }
            set {
                if ((v4 != value)) {
                    v4 = value;
                    FieldModified(WS_metanodes[4]);
                }
            }
        }
        
        public virtual string CompanyUrl {
            get {
                return @__CompanyUrl;
            }
            set {
                @__CompanyUrl = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v5", OriginalFieldName="o5", Name="ContactText")]
        private string @__ContactText {
            get {
                return v5;
            }
            set {
                if ((v5 != value)) {
                    v5 = value;
                    FieldModified(WS_metanodes[5]);
                }
            }
        }
        
        public virtual string ContactText {
            get {
                return @__ContactText;
            }
            set {
                @__ContactText = value;
            }
        }
        
        public static WorkflowServer.Foundation.Documents.FactoryEnvelope<ContactRequest> DefaultFactory {
            get {
                return ((WorkflowServer.Foundation.Documents.FactoryEnvelope<ContactRequest>)(WorkflowServer.Foundation.Documents.DocumentFactory.GetFactoryByName("ContactRequest")));
            }
        }
        
        [System.SerializableAttribute()]
        [System.Runtime.Serialization.DataContractAttribute(Name="messageNode_Node", IsReference=true)]
        public partial class messageNode_Node : WorkflowServer.Foundation.Documents.Extensions.MessageNode {
            
            // Contains names of metanodes. Used at run time by the ElementMetaNode.Build method
            static string[] WS_metanode_names = new string[] {
                    "activity",
                    "type",
                    "suspended",
                    "postedAt",
                    "escalateAt",
                    "escalationTarget"};
            
            // Local metanode index. Populated runtime by the ElmentMetaNode.Build method
            static WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode[] WS_metanodes;
            
            public messageNode_Node(WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode metaNode, WorkflowServer.Foundation.Documents.DocumentNode parent, WorkflowServer.Foundation.Documents.DocumentNode.InitMode initFor) : 
                    base(metaNode, parent, initFor) {
                return;
            }
        }
    }
}
