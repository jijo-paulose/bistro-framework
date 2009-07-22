#pragma warning disable 0169, 0649, 0618
// -------------------------------------------864187d2-c0eb-4620-9c81-c789026db391
//
//		Generated document definition classes
//		Workflow Server code generator v.4.0.1.0
//		All modifications to the content of this file will be lost
//		when file will be overwritten by the code generator
//
//		Generated from the Xml schema located at 
//			Config\Documents\Entities\Posting.xml
//			as of 6/22/2009 2:15 
//
// -------------------------------------------------------------------------------
namespace NoRecruiters3 {
    using WorkflowServer.Foundation.Documents;
    using WorkflowServer.Foundation.Documents.MetaInfo;
    
    
    /// WS_documentDependency WorkflowServer.Foundation.Documents.Document=Unknown
    /// WS_documentDependency NoRecruiters3.Posting=dcd349f9cb7c495494960159ec61ada376a6d15
    /// WS_documentDependency NoRecruiters3.userProfile=435c9b152abf42cb66398bf3d474f7ae899c95
    /// WS_documentDependency NoRecruiters3.userProfile=435c9b152abf42cb66398bf3d474f7ae899c95
    /// WS_documentDependency NoRecruiters3.ContentType=4e2eef1b5ded36252f3997166cdbc219a1a0af
    /// WS_documentDependency NoRecruiters3.Contents=d6a879c36844fba838428e59bd4cff3f4ae9ce1
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="Posting", IsReference=true)]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(tags_NodeCollection))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(applications_NodeCollection))]
    [DocumentVersion("dcd349f9cb7c495494960159ec61ada376a6d15")]
    public partial class Posting : WorkflowServer.Foundation.Documents.Document {
        
        // Contains names of metanodes. Used at run time by the ElementMetaNode.Build method
        static string[] WS_metanode_names = new string[] {
                "docID",
                "tags",
                "applications",
                "User",
                "CreatedOn",
                "LastModifiedOn",
                "ContentType",
                "Heading",
                "ShortName",
                "ShortText",
                "Views",
                "Deleted",
                "Flagged",
                "Published",
                "Active",
                "Contents"};
        
        // Local metanode index. Populated runtime by the ElmentMetaNode.Build method
        static WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode[] WS_metanodes;
        
        public static string WS_documentSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns:wdf=""http://schemas.hill30.com/workflow/documents"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element wdf:read=""s_posting([opt]ShortName)"" wdf:save=""iu_posting"" wdf:delete=""d_posting"" wdf:true-value=""1"" wdf:false-value=""0"" name=""document"">
    <xs:complexType>
      <xs:sequence>
        <xs:element wdf:read=""s_tags"" wdf:save=""iu_tags"" wdf:delete=""d_tags"" maxOccurs=""unbounded"" name=""tags"">
          <xs:complexType>
            <xs:attribute wdf:key-index=""0"" name=""TagId"" type=""xs:int"" />
            <xs:attribute wdf:key-index=""1"" name=""TagText"" type=""xs:string"" />
            <xs:attribute wdf:key-index=""2"" name=""SafeText"" type=""xs:string"" />
          </xs:complexType>
        </xs:element>
        <xs:element wdf:read=""s_application"" wdf:save=""iu_application"" maxOccurs=""unbounded"" name=""applications"">
          <xs:complexType>
            <xs:attribute wdf:refers-to=""Posting"" wdf:key-index=""0"" wdf:param-name=""SubmittedPostingId"" name=""SubmittedPosting"" type=""xs:IDREF"" />
            <xs:attribute name=""SubmittedOn"" type=""xs:dateTime"" />
            <xs:attribute wdf:refers-to=""userProfile"" wdf:key-index=""1"" wdf:param-name=""SubmittedByUserId"" name=""SubmittedBy"" type=""xs:IDREF"" />
            <xs:attribute name=""Comment"" type=""xs:string"" />
          </xs:complexType>
        </xs:element>
      </xs:sequence>
      <xs:attribute wdf:param-name=""PostingId"" wdf:key-index=""0"" name=""docID"" type=""xs:ID"" />
      <xs:attribute wdf:refers-to=""userProfile"" wdf:param-name=""UserId"" name=""User"" type=""xs:IDREF"" />
      <xs:attribute name=""CreatedOn"" type=""xs:dateTime"" />
      <xs:attribute name=""LastModifiedOn"" type=""xs:dateTime"" />
      <xs:attribute wdf:refers-to=""ContentType"" wdf:param-name=""ContentTypeId"" wdf:target-type=""int"" name=""ContentType"" type=""xs:IDREF"" />
      <xs:attribute name=""Heading"" type=""xs:string"" />
      <xs:attribute name=""ShortName"" type=""xs:string"" />
      <xs:attribute name=""ShortText"" type=""xs:string"" />
      <xs:attribute default=""0"" name=""Views"" type=""xs:int"" />
      <xs:attribute default=""false"" name=""Deleted"" type=""xs:boolean"" />
      <xs:attribute default=""false"" name=""Flagged"" type=""xs:boolean"" />
      <xs:attribute default=""false"" name=""Published"" type=""xs:boolean"" />
      <xs:attribute default=""false"" name=""Active"" type=""xs:boolean"" />
      <xs:attribute wdf:refers-to=""Contents"" wdf:param-name=""ContentsId"" name=""Contents"" type=""xs:IDREF"" />
    </xs:complexType>
  </xs:element>
</xs:schema>";
        
        // Private field for the Value value of the the tags document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v1", Order=2001, IsRequired=false)]
        private NoRecruiters3.Posting.tags_NodeCollection v1;
        
        // Private field for the Value value of the the applications document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v2", Order=2002, IsRequired=false)]
        private NoRecruiters3.Posting.applications_NodeCollection v2;
        
        // Private field for the Value value of the the User document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v3", Order=2003, IsRequired=false)]
        private string v3;
        
        // Private field for the Original value of the the User document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o3", Order=3003, IsRequired=false)]
        private string o3;
        
        // Private field for the DocumentReference value of the the User document field
        [System.NonSerializedAttribute()]
        private NoRecruiters3.userProfile r3;
        
        // Private field for the Value value of the the CreatedOn document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v4", Order=2004, IsRequired=false)]
        private System.Nullable<System.DateTime> v4;
        
        // Private field for the Original value of the the CreatedOn document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o4", Order=3004, IsRequired=false)]
        private System.Nullable<System.DateTime> o4;
        
        // Private field for the Value value of the the LastModifiedOn document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v5", Order=2005, IsRequired=false)]
        private System.Nullable<System.DateTime> v5;
        
        // Private field for the Original value of the the LastModifiedOn document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o5", Order=3005, IsRequired=false)]
        private System.Nullable<System.DateTime> o5;
        
        // Private field for the Value value of the the ContentType document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v6", Order=2006, IsRequired=false)]
        private string v6;
        
        // Private field for the Original value of the the ContentType document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o6", Order=3006, IsRequired=false)]
        private string o6;
        
        // Private field for the DocumentReference value of the the ContentType document field
        [System.NonSerializedAttribute()]
        private NoRecruiters3.ContentType r6;
        
        // Private field for the Value value of the the Heading document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v7", Order=2007, IsRequired=false)]
        private string v7;
        
        // Private field for the Original value of the the Heading document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o7", Order=3007, IsRequired=false)]
        private string o7;
        
        // Private field for the Value value of the the ShortName document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v8", Order=2008, IsRequired=false)]
        private string v8;
        
        // Private field for the Original value of the the ShortName document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o8", Order=3008, IsRequired=false)]
        private string o8;
        
        // Private field for the Value value of the the ShortText document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v9", Order=2009, IsRequired=false)]
        private string v9;
        
        // Private field for the Original value of the the ShortText document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o9", Order=3009, IsRequired=false)]
        private string o9;
        
        // Private field for the Value value of the the Views document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v10", Order=2010, IsRequired=false)]
        private System.Nullable<int> v10;
        
        // Private field for the Original value of the the Views document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o10", Order=3010, IsRequired=false)]
        private System.Nullable<int> o10;
        
        // Private field for the Value value of the the Deleted document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v11", Order=2011, IsRequired=false)]
        private System.Nullable<bool> v11;
        
        // Private field for the Original value of the the Deleted document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o11", Order=3011, IsRequired=false)]
        private System.Nullable<bool> o11;
        
        // Private field for the Value value of the the Flagged document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v12", Order=2012, IsRequired=false)]
        private System.Nullable<bool> v12;
        
        // Private field for the Original value of the the Flagged document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o12", Order=3012, IsRequired=false)]
        private System.Nullable<bool> o12;
        
        // Private field for the Value value of the the Published document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v13", Order=2013, IsRequired=false)]
        private System.Nullable<bool> v13;
        
        // Private field for the Original value of the the Published document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o13", Order=3013, IsRequired=false)]
        private System.Nullable<bool> o13;
        
        // Private field for the Value value of the the Active document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v14", Order=2014, IsRequired=false)]
        private System.Nullable<bool> v14;
        
        // Private field for the Original value of the the Active document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o14", Order=3014, IsRequired=false)]
        private System.Nullable<bool> o14;
        
        // Private field for the Value value of the the Contents document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v15", Order=2015, IsRequired=false)]
        private string v15;
        
        // Private field for the Original value of the the Contents document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o15", Order=3015, IsRequired=false)]
        private string o15;
        
        // Private field for the DocumentReference value of the the Contents document field
        [System.NonSerializedAttribute()]
        private NoRecruiters3.Contents r15;
        
        public Posting(WorkflowServer.Foundation.Documents.DocumentFactory factory) : 
                base(factory) {
            return;
        }
        
        public Posting(WorkflowServer.Foundation.Documents.DocumentFactory factory, System.Xml.XmlReader content) : 
                base(factory, content) {
            return;
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(FieldType=typeof(NoRecruiters3.Posting.tags_Node), ValueFieldName="v1")]
        public virtual NoRecruiters3.Posting.tags_NodeCollection tags {
            get {
                return v1;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(FieldType=typeof(NoRecruiters3.Posting.applications_Node), ValueFieldName="v2")]
        public virtual NoRecruiters3.Posting.applications_NodeCollection applications {
            get {
                return v2;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v3", OriginalFieldName="o3", ReferenceFieldName="r3", Name="User")]
        private NoRecruiters3.userProfile @__User {
            get {
                if ((r3 == null)) {
                    // Retrieve the Document
                    r3 = ((NoRecruiters3.userProfile)(GetReferencedDocument(MetaNode[3], new WorkflowServer.Foundation.Documents.DocumentNode.GetterParam("ID", v3))));
                }
                return r3;
            }
            set {
                if ((r3 != value)) {
                    if ((value == null)) {
                        v3 = null;
                    }
                    else {
                        v3 = MetaNode.Factory.ServiceManager.GetLocalID(value.ID);
                    }
                    r3 = value;
                    FieldModified(WS_metanodes[3]);
                }
            }
        }
        
        public virtual NoRecruiters3.userProfile User {
            get {
                return @__User;
            }
            set {
                @__User = value;
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
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v5", OriginalFieldName="o5", Name="LastModifiedOn")]
        private System.Nullable<System.DateTime> @__LastModifiedOn {
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
        
        public virtual System.Nullable<System.DateTime> LastModifiedOn {
            get {
                return @__LastModifiedOn;
            }
            set {
                @__LastModifiedOn = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v6", OriginalFieldName="o6", ReferenceFieldName="r6", Name="ContentType")]
        private NoRecruiters3.ContentType @__ContentType {
            get {
                if ((r6 == null)) {
                    // Retrieve the Document
                    r6 = ((NoRecruiters3.ContentType)(GetReferencedDocument(MetaNode[6], new WorkflowServer.Foundation.Documents.DocumentNode.GetterParam("ID", v6))));
                }
                return r6;
            }
            set {
                if ((r6 != value)) {
                    if ((value == null)) {
                        v6 = null;
                    }
                    else {
                        v6 = MetaNode.Factory.ServiceManager.GetLocalID(value.ID);
                    }
                    r6 = value;
                    FieldModified(WS_metanodes[6]);
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
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v7", OriginalFieldName="o7", Name="Heading")]
        private string @__Heading {
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
        
        public virtual string Heading {
            get {
                return @__Heading;
            }
            set {
                @__Heading = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v8", OriginalFieldName="o8", Name="ShortName")]
        private string @__ShortName {
            get {
                return v8;
            }
            set {
                if ((v8 != value)) {
                    v8 = value;
                    FieldModified(WS_metanodes[8]);
                }
            }
        }
        
        public virtual string ShortName {
            get {
                return @__ShortName;
            }
            set {
                @__ShortName = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v9", OriginalFieldName="o9", Name="ShortText")]
        private string @__ShortText {
            get {
                return v9;
            }
            set {
                if ((v9 != value)) {
                    v9 = value;
                    FieldModified(WS_metanodes[9]);
                }
            }
        }
        
        public virtual string ShortText {
            get {
                return @__ShortText;
            }
            set {
                @__ShortText = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v10", OriginalFieldName="o10", Name="Views")]
        private System.Nullable<int> @__Views {
            get {
                return v10;
            }
            set {
                if ((v10 != value)) {
                    v10 = value;
                    FieldModified(WS_metanodes[10]);
                }
            }
        }
        
        public virtual System.Nullable<int> Views {
            get {
                return @__Views;
            }
            set {
                @__Views = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v11", OriginalFieldName="o11", Name="Deleted")]
        private System.Nullable<bool> @__Deleted {
            get {
                return v11;
            }
            set {
                if ((v11 != value)) {
                    v11 = value;
                    FieldModified(WS_metanodes[11]);
                }
            }
        }
        
        public virtual System.Nullable<bool> Deleted {
            get {
                return @__Deleted;
            }
            set {
                @__Deleted = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v12", OriginalFieldName="o12", Name="Flagged")]
        private System.Nullable<bool> @__Flagged {
            get {
                return v12;
            }
            set {
                if ((v12 != value)) {
                    v12 = value;
                    FieldModified(WS_metanodes[12]);
                }
            }
        }
        
        public virtual System.Nullable<bool> Flagged {
            get {
                return @__Flagged;
            }
            set {
                @__Flagged = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v13", OriginalFieldName="o13", Name="Published")]
        private System.Nullable<bool> @__Published {
            get {
                return v13;
            }
            set {
                if ((v13 != value)) {
                    v13 = value;
                    FieldModified(WS_metanodes[13]);
                }
            }
        }
        
        public virtual System.Nullable<bool> Published {
            get {
                return @__Published;
            }
            set {
                @__Published = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v14", OriginalFieldName="o14", Name="Active")]
        private System.Nullable<bool> @__Active {
            get {
                return v14;
            }
            set {
                if ((v14 != value)) {
                    v14 = value;
                    FieldModified(WS_metanodes[14]);
                }
            }
        }
        
        public virtual System.Nullable<bool> Active {
            get {
                return @__Active;
            }
            set {
                @__Active = value;
            }
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v15", OriginalFieldName="o15", ReferenceFieldName="r15", Name="Contents")]
        private NoRecruiters3.Contents @__Contents {
            get {
                if ((r15 == null)) {
                    // Retrieve the Document
                    r15 = ((NoRecruiters3.Contents)(GetReferencedDocument(MetaNode[15], new WorkflowServer.Foundation.Documents.DocumentNode.GetterParam("ID", v15))));
                }
                return r15;
            }
            set {
                if ((r15 != value)) {
                    if ((value == null)) {
                        v15 = null;
                    }
                    else {
                        v15 = MetaNode.Factory.ServiceManager.GetLocalID(value.ID);
                    }
                    r15 = value;
                    FieldModified(WS_metanodes[15]);
                }
            }
        }
        
        public virtual NoRecruiters3.Contents Contents {
            get {
                return @__Contents;
            }
            set {
                @__Contents = value;
            }
        }
        
        public static WorkflowServer.Foundation.Documents.Factories.StoredProcedure.Factory<Posting> DefaultFactory {
            get {
                return ((WorkflowServer.Foundation.Documents.Factories.StoredProcedure.Factory<Posting>)(WorkflowServer.Foundation.Documents.DocumentFactory.GetFactoryByName("Posting")));
            }
        }
        
        [System.SerializableAttribute()]
        [System.Runtime.Serialization.DataContractAttribute(Name="tags_NodeCollection", IsReference=true)]
        [System.Runtime.Serialization.KnownTypeAttribute(typeof(NoRecruiters3.Posting.tags_Node))]
        public partial class tags_NodeCollection : WorkflowServer.Foundation.Documents.DocumentNodeCollection {
            
            public tags_NodeCollection(WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode metaNode, WorkflowServer.Foundation.Documents.DocumentNode parent) : 
                    base(metaNode, parent) {
            }
            
            public virtual NoRecruiters3.Posting.tags_Node AppendNode() {
                return ((NoRecruiters3.Posting.tags_Node)(base.AppendNode()));
            }
            
            public virtual NoRecruiters3.Posting.tags_Node InsertNodeBefore(NoRecruiters3.Posting.tags_Node sibling) {
                return ((NoRecruiters3.Posting.tags_Node)(base.InsertNodeBefore(sibling)));
            }
        }
        
        [System.SerializableAttribute()]
        [System.Runtime.Serialization.DataContractAttribute(Name="tags_Node", IsReference=true)]
        public partial class tags_Node : WorkflowServer.Foundation.Documents.DocumentNode {
            
            // Contains names of metanodes. Used at run time by the ElementMetaNode.Build method
            static string[] WS_metanode_names = new string[] {
                    "TagId",
                    "TagText",
                    "SafeText"};
            
            // Local metanode index. Populated runtime by the ElmentMetaNode.Build method
            static WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode[] WS_metanodes;
            
            // Private field for the Value value of the the TagId document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v0", Order=2000, IsRequired=false)]
            private System.Nullable<int> v0;
            
            // Private field for the Original value of the the TagId document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o0", Order=3000, IsRequired=false)]
            private System.Nullable<int> o0;
            
            // Private field for the Value value of the the TagText document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v1", Order=2001, IsRequired=false)]
            private string v1;
            
            // Private field for the Original value of the the TagText document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o1", Order=3001, IsRequired=false)]
            private string o1;
            
            // Private field for the Value value of the the SafeText document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v2", Order=2002, IsRequired=false)]
            private string v2;
            
            // Private field for the Original value of the the SafeText document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o2", Order=3002, IsRequired=false)]
            private string o2;
            
            public tags_Node(WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode metaNode, WorkflowServer.Foundation.Documents.DocumentNodeCollection collection, WorkflowServer.Foundation.Documents.DocumentNode sibling, WorkflowServer.Foundation.Documents.DocumentNode.InitMode initFor) : 
                    base(metaNode, collection, sibling, initFor) {
                return;
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v0", OriginalFieldName="o0", Name="TagId")]
            private System.Nullable<int> @__TagId {
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
            
            public virtual System.Nullable<int> TagId {
                get {
                    return @__TagId;
                }
                set {
                    @__TagId = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v1", OriginalFieldName="o1", Name="TagText")]
            private string @__TagText {
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
            
            public virtual string TagText {
                get {
                    return @__TagText;
                }
                set {
                    @__TagText = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v2", OriginalFieldName="o2", Name="SafeText")]
            private string @__SafeText {
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
            
            public virtual string SafeText {
                get {
                    return @__SafeText;
                }
                set {
                    @__SafeText = value;
                }
            }
        }
        
        [System.SerializableAttribute()]
        [System.Runtime.Serialization.DataContractAttribute(Name="applications_NodeCollection", IsReference=true)]
        [System.Runtime.Serialization.KnownTypeAttribute(typeof(NoRecruiters3.Posting.applications_Node))]
        public partial class applications_NodeCollection : WorkflowServer.Foundation.Documents.DocumentNodeCollection {
            
            public applications_NodeCollection(WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode metaNode, WorkflowServer.Foundation.Documents.DocumentNode parent) : 
                    base(metaNode, parent) {
            }
            
            public virtual NoRecruiters3.Posting.applications_Node AppendNode() {
                return ((NoRecruiters3.Posting.applications_Node)(base.AppendNode()));
            }
            
            public virtual NoRecruiters3.Posting.applications_Node InsertNodeBefore(NoRecruiters3.Posting.applications_Node sibling) {
                return ((NoRecruiters3.Posting.applications_Node)(base.InsertNodeBefore(sibling)));
            }
        }
        
        [System.SerializableAttribute()]
        [System.Runtime.Serialization.DataContractAttribute(Name="applications_Node", IsReference=true)]
        public partial class applications_Node : WorkflowServer.Foundation.Documents.DocumentNode {
            
            // Contains names of metanodes. Used at run time by the ElementMetaNode.Build method
            static string[] WS_metanode_names = new string[] {
                    "SubmittedPosting",
                    "SubmittedOn",
                    "SubmittedBy",
                    "Comment"};
            
            // Local metanode index. Populated runtime by the ElmentMetaNode.Build method
            static WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode[] WS_metanodes;
            
            // Private field for the Value value of the the SubmittedPosting document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v0", Order=2000, IsRequired=false)]
            private string v0;
            
            // Private field for the Original value of the the SubmittedPosting document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o0", Order=3000, IsRequired=false)]
            private string o0;
            
            // Private field for the DocumentReference value of the the SubmittedPosting document field
            [System.NonSerializedAttribute()]
            private NoRecruiters3.Posting r0;
            
            // Private field for the Value value of the the SubmittedOn document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v1", Order=2001, IsRequired=false)]
            private System.Nullable<System.DateTime> v1;
            
            // Private field for the Original value of the the SubmittedOn document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o1", Order=3001, IsRequired=false)]
            private System.Nullable<System.DateTime> o1;
            
            // Private field for the Value value of the the SubmittedBy document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v2", Order=2002, IsRequired=false)]
            private string v2;
            
            // Private field for the Original value of the the SubmittedBy document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o2", Order=3002, IsRequired=false)]
            private string o2;
            
            // Private field for the DocumentReference value of the the SubmittedBy document field
            [System.NonSerializedAttribute()]
            private NoRecruiters3.userProfile r2;
            
            // Private field for the Value value of the the Comment document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v3", Order=2003, IsRequired=false)]
            private string v3;
            
            // Private field for the Original value of the the Comment document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o3", Order=3003, IsRequired=false)]
            private string o3;
            
            public applications_Node(WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode metaNode, WorkflowServer.Foundation.Documents.DocumentNodeCollection collection, WorkflowServer.Foundation.Documents.DocumentNode sibling, WorkflowServer.Foundation.Documents.DocumentNode.InitMode initFor) : 
                    base(metaNode, collection, sibling, initFor) {
                return;
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v0", OriginalFieldName="o0", ReferenceFieldName="r0", Name="SubmittedPosting")]
            private NoRecruiters3.Posting @__SubmittedPosting {
                get {
                    if ((r0 == null)) {
                        // Retrieve the Document
                        r0 = ((NoRecruiters3.Posting)(GetReferencedDocument(MetaNode[0], new WorkflowServer.Foundation.Documents.DocumentNode.GetterParam("ID", v0))));
                    }
                    return r0;
                }
                set {
                    if ((r0 != value)) {
                        if ((value == null)) {
                            v0 = null;
                        }
                        else {
                            v0 = MetaNode.Factory.ServiceManager.GetLocalID(value.ID);
                        }
                        r0 = value;
                        FieldModified(WS_metanodes[0]);
                    }
                }
            }
            
            public virtual NoRecruiters3.Posting SubmittedPosting {
                get {
                    return @__SubmittedPosting;
                }
                set {
                    @__SubmittedPosting = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v1", OriginalFieldName="o1", Name="SubmittedOn")]
            private System.Nullable<System.DateTime> @__SubmittedOn {
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
            
            public virtual System.Nullable<System.DateTime> SubmittedOn {
                get {
                    return @__SubmittedOn;
                }
                set {
                    @__SubmittedOn = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v2", OriginalFieldName="o2", ReferenceFieldName="r2", Name="SubmittedBy")]
            private NoRecruiters3.userProfile @__SubmittedBy {
                get {
                    if ((r2 == null)) {
                        // Retrieve the Document
                        r2 = ((NoRecruiters3.userProfile)(GetReferencedDocument(MetaNode[2], new WorkflowServer.Foundation.Documents.DocumentNode.GetterParam("ID", v2))));
                    }
                    return r2;
                }
                set {
                    if ((r2 != value)) {
                        if ((value == null)) {
                            v2 = null;
                        }
                        else {
                            v2 = MetaNode.Factory.ServiceManager.GetLocalID(value.ID);
                        }
                        r2 = value;
                        FieldModified(WS_metanodes[2]);
                    }
                }
            }
            
            public virtual NoRecruiters3.userProfile SubmittedBy {
                get {
                    return @__SubmittedBy;
                }
                set {
                    @__SubmittedBy = value;
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
        }
    }
}
