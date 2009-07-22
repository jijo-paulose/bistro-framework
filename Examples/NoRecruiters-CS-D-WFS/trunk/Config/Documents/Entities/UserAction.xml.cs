#pragma warning disable 0169, 0649, 0618
// -------------------------------------------864187d2-c0eb-4620-9c81-c789026db391
//
//		Generated document definition classes
//		Workflow Server code generator v.4.0.1.0
//		All modifications to the content of this file will be lost
//		when file will be overwritten by the code generator
//
//		Generated from the Xml schema located at 
//			Config\Documents\Entities\UserAction.xml
//			as of 6/22/2009 2:15 
//
// -------------------------------------------------------------------------------
namespace NoRecruiters3 {
    using WorkflowServer.Foundation.Documents;
    using WorkflowServer.Foundation.Documents.MetaInfo;
    
    
    /// WS_documentDependency WorkflowServer.Foundation.Documents.Document=Unknown
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="UserAction", IsReference=true)]
    [DocumentVersion("ceb6fb86fba247d1914da35de4e746faf24ca0")]
    public partial class UserAction : WorkflowServer.Foundation.Documents.Document {
        
        // Contains names of metanodes. Used at run time by the ElementMetaNode.Build method
        static string[] WS_metanode_names = new string[] {
                "docID",
                "UserId",
                "ActionId",
                "Comment",
                "CreatedOn",
                "PostingId"};
        
        // Local metanode index. Populated runtime by the ElmentMetaNode.Build method
        static WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode[] WS_metanodes;
        
        public static string WS_documentSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns:wdf=""http://schemas.hill30.com/workflow/documents"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element wdf:read=""(none)"" wdf:save=""i_user_action"" name=""document"">
    <xs:complexType>
      <xs:attribute wdf:param-name=""UserActionId"" name=""docID"" type=""xs:ID"" />
      <xs:attribute name=""UserId"" type=""xs:string"" />
      <xs:attribute name=""ActionId"" type=""xs:string"" />
      <xs:attribute name=""Comment"" type=""xs:string"" />
      <xs:attribute name=""CreatedOn"" type=""xs:dateTime"" />
      <xs:attribute name=""PostingId"" type=""xs:string"" />
    </xs:complexType>
  </xs:element>
</xs:schema>";
        
        // Private field for the Value value of the the UserId document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v1", Order=2001, IsRequired=false)]
        private string v1;
        
        // Private field for the Original value of the the UserId document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o1", Order=3001, IsRequired=false)]
        private string o1;
        
        // Private field for the Value value of the the ActionId document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v2", Order=2002, IsRequired=false)]
        private string v2;
        
        // Private field for the Original value of the the ActionId document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o2", Order=3002, IsRequired=false)]
        private string o2;
        
        // Private field for the Value value of the the Comment document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v3", Order=2003, IsRequired=false)]
        private string v3;
        
        // Private field for the Original value of the the Comment document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o3", Order=3003, IsRequired=false)]
        private string o3;
        
        // Private field for the Value value of the the CreatedOn document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v4", Order=2004, IsRequired=false)]
        private System.Nullable<System.DateTime> v4;
        
        // Private field for the Original value of the the CreatedOn document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o4", Order=3004, IsRequired=false)]
        private System.Nullable<System.DateTime> o4;
        
        // Private field for the Value value of the the PostingId document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v5", Order=2005, IsRequired=false)]
        private string v5;
        
        // Private field for the Original value of the the PostingId document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o5", Order=3005, IsRequired=false)]
        private string o5;
        
        public UserAction(WorkflowServer.Foundation.Documents.DocumentFactory factory) : 
                base(factory) {
            return;
        }
        
        public UserAction(WorkflowServer.Foundation.Documents.DocumentFactory factory, System.Xml.XmlReader content) : 
                base(factory, content) {
            return;
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v1", OriginalFieldName="o1", Name="UserId")]
        private string @__UserId {
            get {
                return v1;
            }
            set {
                if ((v1 != value)) {
                    v1 = value;
                    FieldModified(WS_metanodes[1]);
                }
            }
        }
        
        public virtual string UserId {
            get {
                return @__UserId;
            }
            set {
                @__UserId = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v2", OriginalFieldName="o2", Name="ActionId")]
        private string @__ActionId {
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
        
        public virtual string ActionId {
            get {
                return @__ActionId;
            }
            set {
                @__ActionId = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v3", OriginalFieldName="o3", Name="Comment")]
        private string @__Comment {
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
        
        public virtual string Comment {
            get {
                return @__Comment;
            }
            set {
                @__Comment = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v4", OriginalFieldName="o4", Name="CreatedOn")]
        private System.Nullable<System.DateTime> @__CreatedOn {
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
        
        public virtual System.Nullable<System.DateTime> CreatedOn {
            get {
                return @__CreatedOn;
            }
            set {
                @__CreatedOn = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v5", OriginalFieldName="o5", Name="PostingId")]
        private string @__PostingId {
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
        
        public virtual string PostingId {
            get {
                return @__PostingId;
            }
            set {
                @__PostingId = value;
            }
        }
        
        public static WorkflowServer.Foundation.Documents.Factories.StoredProcedure.Factory<UserAction> DefaultFactory {
            get {
                return ((WorkflowServer.Foundation.Documents.Factories.StoredProcedure.Factory<UserAction>)(WorkflowServer.Foundation.Documents.DocumentFactory.GetFactoryByName("UserAction")));
            }
        }
    }
}
