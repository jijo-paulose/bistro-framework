#pragma warning disable 0169, 0649, 0618
// -------------------------------------------864187d2-c0eb-4620-9c81-c789026db391
//
//		Generated document definition classes
//		Workflow Server code generator v.4.0.1.0
//		All modifications to the content of this file will be lost
//		when file will be overwritten by the code generator
//
//		Generated from the Xml schema located at 
//			Config\Documents\System\web-session.xml
//			as of 6/22/2009 2:15 
//
// -------------------------------------------------------------------------------
namespace NoRecruiters3 {
    using WorkflowServer.Foundation.Documents;
    using WorkflowServer.Foundation.Documents.MetaInfo;
    
    
    /// WS_documentDependency WorkflowServer.Foundation.Documents.Session=Unknown
    /// WS_documentDependency NoRecruiters3.userProfile=435c9b152abf42cb66398bf3d474f7ae899c95
    /// WS_documentDependency NoRecruiters3.ContentType=4e2eef1b5ded36252f3997166cdbc219a1a0af
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="web_session", IsReference=true)]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(currentTags_NodeCollection))]
    [DocumentVersion("cb7cc9fd1dacee796bd06684d09cbddc439d94")]
    public partial class web_session : WorkflowServer.Foundation.Documents.Session {
        
        // Contains names of metanodes. Used at run time by the ElementMetaNode.Build method
        static string[] WS_metanode_names = new string[] {
                "docID",
                "messageNode",
                "UserProfile",
                "currentTags",
                "SearchByTag",
                "SearchByQuery",
                "RequestedStaticContent",
                "ContentType"};
        
        // Local metanode index. Populated runtime by the ElmentMetaNode.Build method
        static WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode[] WS_metanodes;
        
        public static string WS_documentSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns:sql=""urn:schemas-microsoft-com:mapping-schema"" xmlns:wdf=""http://schemas.hill30.com/workflow/documents"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element name=""document"">
    <xs:complexType>
      <xs:sequence>
        <xs:element maxOccurs=""unbounded"" name=""currentTags"">
          <xs:complexType>
            <xs:attribute name=""tag"" type=""xs:string"" />
          </xs:complexType>
        </xs:element>
      </xs:sequence>
      <xs:attribute wdf:refers-to=""userProfile"" name=""UserProfile"" type=""xs:IDREF"" />
      <xs:attribute name=""SearchByTag"" type=""xs:string"" />
      <xs:attribute name=""SearchByQuery"" type=""xs:string"" />
      <xs:attribute name=""RequestedStaticContent"" type=""xs:string"" />
      <xs:attribute wdf:refers-to=""ContentType"" name=""ContentType"" type=""xs:IDREF"" />
    </xs:complexType>
  </xs:element>
</xs:schema>";
        
        // Private field for the Value value of the the currentTags document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v3", Order=2003, IsRequired=false)]
        private NoRecruiters3.web_session.currentTags_NodeCollection v3;
        
        // Private field for the Value value of the the SearchByTag document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v4", Order=2004, IsRequired=false)]
        private string v4;
        
        // Private field for the Original value of the the SearchByTag document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o4", Order=3004, IsRequired=false)]
        private string o4;
        
        // Private field for the Value value of the the SearchByQuery document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v5", Order=2005, IsRequired=false)]
        private string v5;
        
        // Private field for the Original value of the the SearchByQuery document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o5", Order=3005, IsRequired=false)]
        private string o5;
        
        // Private field for the Value value of the the RequestedStaticContent document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v6", Order=2006, IsRequired=false)]
        private string v6;
        
        // Private field for the Original value of the the RequestedStaticContent document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o6", Order=3006, IsRequired=false)]
        private string o6;
        
        // Private field for the Value value of the the ContentType document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v7", Order=2007, IsRequired=false)]
        private string v7;
        
        // Private field for the Original value of the the ContentType document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o7", Order=3007, IsRequired=false)]
        private string o7;
        
        // Private field for the DocumentReference value of the the ContentType document field
        [System.NonSerializedAttribute()]
        private NoRecruiters3.ContentType r7;
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(FieldType=typeof(NoRecruiters3.web_session.currentTags_Node), ValueFieldName="v3")]
        public virtual NoRecruiters3.web_session.currentTags_NodeCollection currentTags {
            get {
                return v3;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute()]
        public new virtual NoRecruiters3.userProfile UserProfile {
            get {
                return ((NoRecruiters3.userProfile)(base.UserProfile));
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v4", OriginalFieldName="o4", Name="SearchByTag")]
        private string @__SearchByTag {
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
        
        public virtual string SearchByTag {
            get {
                return @__SearchByTag;
            }
            set {
                @__SearchByTag = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v5", OriginalFieldName="o5", Name="SearchByQuery")]
        private string @__SearchByQuery {
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
        
        public virtual string SearchByQuery {
            get {
                return @__SearchByQuery;
            }
            set {
                @__SearchByQuery = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v6", OriginalFieldName="o6", Name="RequestedStaticContent")]
        private string @__RequestedStaticContent {
            get {
                return v6;
            }
            set {
                if ((v6 != value)) {
                    v6 = value;
                    FieldModified(WS_metanodes[6]);
                }
            }
        }
        
        public virtual string RequestedStaticContent {
            get {
                return @__RequestedStaticContent;
            }
            set {
                @__RequestedStaticContent = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v7", OriginalFieldName="o7", ReferenceFieldName="r7", Name="ContentType")]
        private NoRecruiters3.ContentType @__ContentType {
            get {
                if ((r7 == null)) {
                    // Retrieve the Document
                    r7 = ((NoRecruiters3.ContentType)(GetReferencedDocument(MetaNode[7], new WorkflowServer.Foundation.Documents.DocumentNode.GetterParam("ID", v7))));
                }
                return r7;
            }
            set {
                if ((r7 != value)) {
                    if ((value == null)) {
                        v7 = null;
                    }
                    else {
                        v7 = MetaNode.Factory.ServiceManager.GetLocalID(value.ID);
                    }
                    r7 = value;
                    FieldModified(WS_metanodes[7]);
                }
            }
        }
        
        public virtual NoRecruiters3.ContentType ContentType {
            get {
                return @__ContentType;
            }
            set {
                @__ContentType = value;
            }
        }
        
        public static WorkflowServer.Foundation.Documents.FactoryEnvelope<web_session> DefaultFactory {
            get {
                return ((WorkflowServer.Foundation.Documents.FactoryEnvelope<web_session>)(WorkflowServer.Foundation.Documents.DocumentFactory.GetFactoryByName("web-session")));
            }
        }
        
        [System.SerializableAttribute()]
        [System.Runtime.Serialization.DataContractAttribute(Name="currentTags_NodeCollection", IsReference=true)]
        [System.Runtime.Serialization.KnownTypeAttribute(typeof(NoRecruiters3.web_session.currentTags_Node))]
        public partial class currentTags_NodeCollection : WorkflowServer.Foundation.Documents.DocumentNodeCollection {
            
            public currentTags_NodeCollection(WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode metaNode, WorkflowServer.Foundation.Documents.DocumentNode parent) : 
                    base(metaNode, parent) {
            }
            
            public virtual NoRecruiters3.web_session.currentTags_Node AppendNode() {
                return ((NoRecruiters3.web_session.currentTags_Node)(base.AppendNode()));
            }
            
            public virtual NoRecruiters3.web_session.currentTags_Node InsertNodeBefore(NoRecruiters3.web_session.currentTags_Node sibling) {
                return ((NoRecruiters3.web_session.currentTags_Node)(base.InsertNodeBefore(sibling)));
            }
        }
        
        [System.SerializableAttribute()]
        [System.Runtime.Serialization.DataContractAttribute(Name="currentTags_Node", IsReference=true)]
        public partial class currentTags_Node : WorkflowServer.Foundation.Documents.DocumentNode {
            
            // Contains names of metanodes. Used at run time by the ElementMetaNode.Build method
            static string[] WS_metanode_names = new string[] {
                    "tag"};
            
            // Local metanode index. Populated runtime by the ElmentMetaNode.Build method
            static WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode[] WS_metanodes;
            
            // Private field for the Value value of the the tag document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v0", Order=2000, IsRequired=false)]
            private string v0;
            
            // Private field for the Original value of the the tag document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o0", Order=3000, IsRequired=false)]
            private string o0;
            
            public currentTags_Node(WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode metaNode, WorkflowServer.Foundation.Documents.DocumentNodeCollection collection, WorkflowServer.Foundation.Documents.DocumentNode sibling, WorkflowServer.Foundation.Documents.DocumentNode.InitMode initFor) : 
                    base(metaNode, collection, sibling, initFor) {
                return;
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v0", OriginalFieldName="o0", Name="tag")]
            private string @__tag {
                get {
                    return v0;
                }
                set {
                    if ((v0 != value)) {
                        v0 = value;
                        FieldModified(WS_metanodes[0]);
                    }
                }
            }
            
            public virtual string tag {
                get {
                    return @__tag;
                }
                set {
                    @__tag = value;
                }
            }
        }
    }
}
