#pragma warning disable 0169, 0649, 0618
// -------------------------------------------864187d2-c0eb-4620-9c81-c789026db391
//
//		Generated document definition classes
//		Workflow Server code generator v.4.0.1.0
//		All modifications to the content of this file will be lost
//		when file will be overwritten by the code generator
//
//		Generated from the Xml schema located at 
//			Config\Documents\System\userProfile.xml
//			as of 6/22/2009 2:15 
//
// -------------------------------------------------------------------------------
namespace NoRecruiters3 {
    using WorkflowServer.Foundation.Documents;
    using WorkflowServer.Foundation.Documents.MetaInfo;
    
    
    /// WS_documentDependency WorkflowServer.Foundation.Documents.UserProfile=Unknown
    /// WS_documentDependency NoRecruiters3.Posting=dcd349f9cb7c495494960159ec61ada376a6d15
    /// WS_documentDependency NoRecruiters3.UserType=5cc555583cd88696cda8944eb5c2b1136708dc6
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="userProfile", IsReference=true)]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(role_NodeCollection))]
    [DocumentVersion("435c9b152abf42cb66398bf3d474f7ae899c95")]
    public partial class userProfile : WorkflowServer.Foundation.Documents.UserProfile {
        
        // Contains names of metanodes. Used at run time by the ElementMetaNode.Build method
        static string[] WS_metanode_names = new string[] {
                "docID",
                "role",
                "password",
                "name",
                "tokenID",
                "Email",
                "FirstName",
                "LastName",
                "Posting",
                "UserType"};
        
        // Local metanode index. Populated runtime by the ElmentMetaNode.Build method
        static WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode[] WS_metanodes;
        
        public static string WS_documentSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns:wdf=""http://schemas.hill30.com/workflow/documents"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element wdf:read=""s_user"" wdf:save=""iu_user"" name=""document"">
    <xs:complexType>
      <xs:sequence>
        <xs:element wdf:transient=""true"" minOccurs=""0"" maxOccurs=""unbounded"" name=""role"">
          <xs:complexType>
            <xs:attribute name=""name"" type=""xs:string"" />
          </xs:complexType>
        </xs:element>
      </xs:sequence>
      <xs:attribute wdf:param-name=""UserId"" wdf:param-index=""0"" name=""docID"" type=""xs:ID"" />
      <xs:attribute wdf:param-name=""UserName"" name=""name"" type=""xs:string"" />
      <xs:attribute wdf:param-name=""Password"" name=""password"" type=""xs:string"" />
      <xs:attribute name=""Email"" type=""xs:string"" />
      <xs:attribute name=""FirstName"" type=""xs:string"" />
      <xs:attribute name=""LastName"" type=""xs:string"" />
      <xs:attribute wdf:refers-to=""Posting"" wdf:param-name=""PostingId"" name=""Posting"" type=""xs:IDREF"" />
      <xs:attribute wdf:refers-to=""UserType"" wdf:param-name=""UserTypeId"" name=""UserType"" type=""xs:IDREF"" />
    </xs:complexType>
  </xs:element>
</xs:schema>";
        
        // Private field for the Value value of the the Email document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v5", Order=2005, IsRequired=false)]
        private string v5;
        
        // Private field for the Original value of the the Email document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o5", Order=3005, IsRequired=false)]
        private string o5;
        
        // Private field for the Value value of the the FirstName document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v6", Order=2006, IsRequired=false)]
        private string v6;
        
        // Private field for the Original value of the the FirstName document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o6", Order=3006, IsRequired=false)]
        private string o6;
        
        // Private field for the Value value of the the LastName document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v7", Order=2007, IsRequired=false)]
        private string v7;
        
        // Private field for the Original value of the the LastName document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o7", Order=3007, IsRequired=false)]
        private string o7;
        
        // Private field for the Value value of the the Posting document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v8", Order=2008, IsRequired=false)]
        private string v8;
        
        // Private field for the Original value of the the Posting document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o8", Order=3008, IsRequired=false)]
        private string o8;
        
        // Private field for the DocumentReference value of the the Posting document field
        [System.NonSerializedAttribute()]
        private NoRecruiters3.Posting r8;
        
        // Private field for the Value value of the the UserType document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v9", Order=2009, IsRequired=false)]
        private string v9;
        
        // Private field for the Original value of the the UserType document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o9", Order=3009, IsRequired=false)]
        private string o9;
        
        // Private field for the DocumentReference value of the the UserType document field
        [System.NonSerializedAttribute()]
        private NoRecruiters3.UserType r9;
        
        public userProfile(WorkflowServer.Foundation.Documents.DocumentFactory factory) : 
                base(factory) {
            return;
        }
        
        public userProfile(WorkflowServer.Foundation.Documents.DocumentFactory factory, WorkflowServer.Foundation.Documents.Session session) : 
                base(factory, session) {
            return;
        }
        
        public userProfile(WorkflowServer.Foundation.Documents.DocumentFactory factory, System.Xml.XmlReader content) : 
                base(factory, content) {
            return;
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(FieldType=typeof(NoRecruiters3.userProfile.role_Node))]
        public new virtual NoRecruiters3.userProfile.role_NodeCollection role {
            get {
                return ((NoRecruiters3.userProfile.role_NodeCollection)(base.role));
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v5", OriginalFieldName="o5", Name="Email")]
        private string @__Email {
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
        
        public virtual string Email {
            get {
                return @__Email;
            }
            set {
                @__Email = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v6", OriginalFieldName="o6", Name="FirstName")]
        private string @__FirstName {
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
        
        public virtual string FirstName {
            get {
                return @__FirstName;
            }
            set {
                @__FirstName = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v7", OriginalFieldName="o7", Name="LastName")]
        private string @__LastName {
            get {
                return v7;
            }
            set {
                if ((v7 != value)) {
                    v7 = value;
                    FieldModified(WS_metanodes[7]);
                }
            }
        }
        
        public virtual string LastName {
            get {
                return @__LastName;
            }
            set {
                @__LastName = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v8", OriginalFieldName="o8", ReferenceFieldName="r8", Name="Posting")]
        private NoRecruiters3.Posting @__Posting {
            get {
                if ((r8 == null)) {
                    // Retrieve the Document
                    r8 = ((NoRecruiters3.Posting)(GetReferencedDocument(MetaNode[8], new WorkflowServer.Foundation.Documents.DocumentNode.GetterParam("ID", v8))));
                }
                return r8;
            }
            set {
                if ((r8 != value)) {
                    if ((value == null)) {
                        v8 = null;
                    }
                    else {
                        v8 = MetaNode.Factory.ServiceManager.GetLocalID(value.ID);
                    }
                    r8 = value;
                    FieldModified(WS_metanodes[8]);
                }
            }
        }
        
        public virtual NoRecruiters3.Posting Posting {
            get {
                return @__Posting;
            }
            set {
                @__Posting = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v9", OriginalFieldName="o9", ReferenceFieldName="r9", Name="UserType")]
        private NoRecruiters3.UserType @__UserType {
            get {
                if ((r9 == null)) {
                    // Retrieve the Document
                    r9 = ((NoRecruiters3.UserType)(GetReferencedDocument(MetaNode[9], new WorkflowServer.Foundation.Documents.DocumentNode.GetterParam("ID", v9))));
                }
                return r9;
            }
            set {
                if ((r9 != value)) {
                    if ((value == null)) {
                        v9 = null;
                    }
                    else {
                        v9 = MetaNode.Factory.ServiceManager.GetLocalID(value.ID);
                    }
                    r9 = value;
                    FieldModified(WS_metanodes[9]);
                }
            }
        }
        
        public virtual NoRecruiters3.UserType UserType {
            get {
                return @__UserType;
            }
            set {
                @__UserType = value;
            }
        }
        
        public static WorkflowServer.Foundation.Documents.Factories.StoredProcedure.Factory<userProfile> DefaultFactory {
            get {
                return ((WorkflowServer.Foundation.Documents.Factories.StoredProcedure.Factory<userProfile>)(WorkflowServer.Foundation.Documents.DocumentFactory.GetFactoryByName("userProfile")));
            }
        }
        
        [System.SerializableAttribute()]
        [System.Runtime.Serialization.DataContractAttribute(Name="role_NodeCollection", IsReference=true)]
        [System.Runtime.Serialization.KnownTypeAttribute(typeof(NoRecruiters3.userProfile.role_Node))]
        public partial class role_NodeCollection : WorkflowServer.Foundation.Documents.DocumentNodeCollection {
            
            public role_NodeCollection(WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode metaNode, WorkflowServer.Foundation.Documents.DocumentNode parent) : 
                    base(metaNode, parent) {
            }
            
            public virtual NoRecruiters3.userProfile.role_Node AppendNode() {
                return ((NoRecruiters3.userProfile.role_Node)(base.AppendNode()));
            }
            
            public virtual NoRecruiters3.userProfile.role_Node InsertNodeBefore(NoRecruiters3.userProfile.role_Node sibling) {
                return ((NoRecruiters3.userProfile.role_Node)(base.InsertNodeBefore(sibling)));
            }
        }
        
        [System.SerializableAttribute()]
        [System.Runtime.Serialization.DataContractAttribute(Name="role_Node", IsReference=true)]
        public partial class role_Node : WorkflowServer.Foundation.Documents.Extensions.Role {
            
            // Contains names of metanodes. Used at run time by the ElementMetaNode.Build method
            static string[] WS_metanode_names = new string[] {
                    "name"};
            
            // Local metanode index. Populated runtime by the ElmentMetaNode.Build method
            static WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode[] WS_metanodes;
            
            public role_Node(WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode metaNode, WorkflowServer.Foundation.Documents.DocumentNodeCollection collection, WorkflowServer.Foundation.Documents.DocumentNode sibling, WorkflowServer.Foundation.Documents.DocumentNode.InitMode initFor) : 
                    base(metaNode, collection, sibling, initFor) {
                return;
            }
        }
    }
}
