#pragma warning disable 0169, 0649, 0618
// -------------------------------------------864187d2-c0eb-4620-9c81-c789026db391
//
//		Generated document definition classes
//		Workflow Server code generator v.4.0.1.0
//		All modifications to the content of this file will be lost
//		when file will be overwritten by the code generator
//
//		Generated from the Xml schema located at 
//			Config\Documents\Searches\SearchPostings.xml
//			as of 6/22/2009 2:15 
//
// -------------------------------------------------------------------------------
namespace NoRecruiters3 {
    using WorkflowServer.Foundation.Documents;
    using WorkflowServer.Foundation.Documents.MetaInfo;
    
    
    /// WS_documentDependency WorkflowServer.Foundation.Documents.Document=Unknown
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="SearchPostings", IsReference=true)]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(data_NodeCollection))]
    [DocumentVersion("d13a452da13a97197d8f4fc4b736c1654bd18271")]
    public partial class SearchPostings : WorkflowServer.Foundation.Documents.Document {
        
        // Contains names of metanodes. Used at run time by the ElementMetaNode.Build method
        static string[] WS_metanode_names = new string[] {
                "docID",
                "data"};
        
        // Local metanode index. Populated runtime by the ElmentMetaNode.Build method
        static WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode[] WS_metanodes;
        
        public static string WS_documentSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns:wdf=""http://schemas.hill30.com/workflow/documents"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element wdf:transient=""true"" wdf:skip-extra=""true"" name=""document"">
    <xs:complexType>
      <xs:sequence>
        <xs:element wdf:read=""s_postings_by_query(content_type, [opt]tag_list, [opt]query, precision)"" maxOccurs=""unbounded"" name=""data"">
          <xs:complexType>
            <xs:attribute name=""PostingId"" type=""xs:string"" />
            <xs:attribute name=""CreatedOn"" type=""xs:dateTime"" />
            <xs:attribute name=""LastModifiedOn"" type=""xs:dateTime"" />
            <xs:attribute name=""ContentType"" type=""xs:string"" />
            <xs:attribute name=""Heading"" type=""xs:string"" />
            <xs:attribute name=""ShortName"" type=""xs:string"" />
            <xs:attribute name=""ShortText"" type=""xs:string"" />
            <xs:attribute name=""Views"" type=""xs:int"" />
            <xs:attribute name=""Deleted"" type=""xs:boolean"" />
            <xs:attribute name=""Flagged"" type=""xs:boolean"" />
            <xs:attribute name=""Published"" type=""xs:boolean"" />
            <xs:attribute name=""Active"" type=""xs:boolean"" />
            <xs:attribute name=""Rank"" type=""xs:int"" />
          </xs:complexType>
        </xs:element>
      </xs:sequence>
      <xs:attribute wdf:transient=""true"" name=""docID"" type=""xs:ID"" />
    </xs:complexType>
  </xs:element>
</xs:schema>";
        
        // Private field for the Value value of the the data document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v1", Order=2001, IsRequired=false)]
        private NoRecruiters3.SearchPostings.data_NodeCollection v1;
        
        public SearchPostings(WorkflowServer.Foundation.Documents.DocumentFactory factory) : 
                base(factory) {
            return;
        }
        
        public SearchPostings(WorkflowServer.Foundation.Documents.DocumentFactory factory, System.Xml.XmlReader content) : 
                base(factory, content) {
            return;
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(FieldType=typeof(NoRecruiters3.SearchPostings.data_Node), ValueFieldName="v1")]
        public virtual NoRecruiters3.SearchPostings.data_NodeCollection data {
            get {
                return v1;
            }
        }
        
        public static WorkflowServer.Foundation.Documents.Factories.StoredProcedure.Factory<SearchPostings> DefaultFactory {
            get {
                return ((WorkflowServer.Foundation.Documents.Factories.StoredProcedure.Factory<SearchPostings>)(WorkflowServer.Foundation.Documents.DocumentFactory.GetFactoryByName("SearchPostings")));
            }
        }
        
        [System.SerializableAttribute()]
        [System.Runtime.Serialization.DataContractAttribute(Name="data_NodeCollection", IsReference=true)]
        [System.Runtime.Serialization.KnownTypeAttribute(typeof(NoRecruiters3.SearchPostings.data_Node))]
        public partial class data_NodeCollection : WorkflowServer.Foundation.Documents.DocumentNodeCollection {
            
            public data_NodeCollection(WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode metaNode, WorkflowServer.Foundation.Documents.DocumentNode parent) : 
                    base(metaNode, parent) {
            }
            
            public virtual NoRecruiters3.SearchPostings.data_Node AppendNode() {
                return ((NoRecruiters3.SearchPostings.data_Node)(base.AppendNode()));
            }
            
            public virtual NoRecruiters3.SearchPostings.data_Node InsertNodeBefore(NoRecruiters3.SearchPostings.data_Node sibling) {
                return ((NoRecruiters3.SearchPostings.data_Node)(base.InsertNodeBefore(sibling)));
            }
        }
        
        [System.SerializableAttribute()]
        [System.Runtime.Serialization.DataContractAttribute(Name="data_Node", IsReference=true)]
        public partial class data_Node : WorkflowServer.Foundation.Documents.DocumentNode {
            
            // Contains names of metanodes. Used at run time by the ElementMetaNode.Build method
            static string[] WS_metanode_names = new string[] {
                    "PostingId",
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
                    "Rank"};
            
            // Local metanode index. Populated runtime by the ElmentMetaNode.Build method
            static WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode[] WS_metanodes;
            
            // Private field for the Value value of the the PostingId document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v0", Order=2000, IsRequired=false)]
            private string v0;
            
            // Private field for the Original value of the the PostingId document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o0", Order=3000, IsRequired=false)]
            private string o0;
            
            // Private field for the Value value of the the CreatedOn document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v1", Order=2001, IsRequired=false)]
            private System.Nullable<System.DateTime> v1;
            
            // Private field for the Original value of the the CreatedOn document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o1", Order=3001, IsRequired=false)]
            private System.Nullable<System.DateTime> o1;
            
            // Private field for the Value value of the the LastModifiedOn document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v2", Order=2002, IsRequired=false)]
            private System.Nullable<System.DateTime> v2;
            
            // Private field for the Original value of the the LastModifiedOn document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o2", Order=3002, IsRequired=false)]
            private System.Nullable<System.DateTime> o2;
            
            // Private field for the Value value of the the ContentType document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v3", Order=2003, IsRequired=false)]
            private string v3;
            
            // Private field for the Original value of the the ContentType document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o3", Order=3003, IsRequired=false)]
            private string o3;
            
            // Private field for the Value value of the the Heading document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v4", Order=2004, IsRequired=false)]
            private string v4;
            
            // Private field for the Original value of the the Heading document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o4", Order=3004, IsRequired=false)]
            private string o4;
            
            // Private field for the Value value of the the ShortName document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v5", Order=2005, IsRequired=false)]
            private string v5;
            
            // Private field for the Original value of the the ShortName document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o5", Order=3005, IsRequired=false)]
            private string o5;
            
            // Private field for the Value value of the the ShortText document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v6", Order=2006, IsRequired=false)]
            private string v6;
            
            // Private field for the Original value of the the ShortText document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o6", Order=3006, IsRequired=false)]
            private string o6;
            
            // Private field for the Value value of the the Views document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v7", Order=2007, IsRequired=false)]
            private System.Nullable<int> v7;
            
            // Private field for the Original value of the the Views document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o7", Order=3007, IsRequired=false)]
            private System.Nullable<int> o7;
            
            // Private field for the Value value of the the Deleted document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v8", Order=2008, IsRequired=false)]
            private System.Nullable<bool> v8;
            
            // Private field for the Original value of the the Deleted document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o8", Order=3008, IsRequired=false)]
            private System.Nullable<bool> o8;
            
            // Private field for the Value value of the the Flagged document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v9", Order=2009, IsRequired=false)]
            private System.Nullable<bool> v9;
            
            // Private field for the Original value of the the Flagged document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o9", Order=3009, IsRequired=false)]
            private System.Nullable<bool> o9;
            
            // Private field for the Value value of the the Published document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v10", Order=2010, IsRequired=false)]
            private System.Nullable<bool> v10;
            
            // Private field for the Original value of the the Published document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o10", Order=3010, IsRequired=false)]
            private System.Nullable<bool> o10;
            
            // Private field for the Value value of the the Active document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v11", Order=2011, IsRequired=false)]
            private System.Nullable<bool> v11;
            
            // Private field for the Original value of the the Active document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o11", Order=3011, IsRequired=false)]
            private System.Nullable<bool> o11;
            
            // Private field for the Value value of the the Rank document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v12", Order=2012, IsRequired=false)]
            private System.Nullable<int> v12;
            
            // Private field for the Original value of the the Rank document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o12", Order=3012, IsRequired=false)]
            private System.Nullable<int> o12;
            
            public data_Node(WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode metaNode, WorkflowServer.Foundation.Documents.DocumentNodeCollection collection, WorkflowServer.Foundation.Documents.DocumentNode sibling, WorkflowServer.Foundation.Documents.DocumentNode.InitMode initFor) : 
                    base(metaNode, collection, sibling, initFor) {
                return;
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v0", OriginalFieldName="o0", Name="PostingId")]
            private string @__PostingId {
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
            
            public virtual string PostingId {
                get {
                    return @__PostingId;
                }
                set {
                    @__PostingId = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v1", OriginalFieldName="o1", Name="CreatedOn")]
            private System.Nullable<System.DateTime> @__CreatedOn {
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
            
            public virtual System.Nullable<System.DateTime> CreatedOn {
                get {
                    return @__CreatedOn;
                }
                set {
                    @__CreatedOn = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v2", OriginalFieldName="o2", Name="LastModifiedOn")]
            private System.Nullable<System.DateTime> @__LastModifiedOn {
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
            
            public virtual System.Nullable<System.DateTime> LastModifiedOn {
                get {
                    return @__LastModifiedOn;
                }
                set {
                    @__LastModifiedOn = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v3", OriginalFieldName="o3", Name="ContentType")]
            private string @__ContentType {
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
            
            public virtual string ContentType {
                get {
                    return @__ContentType;
                }
                set {
                    @__ContentType = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v4", OriginalFieldName="o4", Name="Heading")]
            private string @__Heading {
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
            
            public virtual string Heading {
                get {
                    return @__Heading;
                }
                set {
                    @__Heading = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v5", OriginalFieldName="o5", Name="ShortName")]
            private string @__ShortName {
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
            
            public virtual string ShortName {
                get {
                    return @__ShortName;
                }
                set {
                    @__ShortName = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v6", OriginalFieldName="o6", Name="ShortText")]
            private string @__ShortText {
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
            
            public virtual string ShortText {
                get {
                    return @__ShortText;
                }
                set {
                    @__ShortText = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v7", OriginalFieldName="o7", Name="Views")]
            private System.Nullable<int> @__Views {
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
            
            public virtual System.Nullable<int> Views {
                get {
                    return @__Views;
                }
                set {
                    @__Views = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v8", OriginalFieldName="o8", Name="Deleted")]
            private System.Nullable<bool> @__Deleted {
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
            
            public virtual System.Nullable<bool> Deleted {
                get {
                    return @__Deleted;
                }
                set {
                    @__Deleted = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v9", OriginalFieldName="o9", Name="Flagged")]
            private System.Nullable<bool> @__Flagged {
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
            
            public virtual System.Nullable<bool> Flagged {
                get {
                    return @__Flagged;
                }
                set {
                    @__Flagged = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v10", OriginalFieldName="o10", Name="Published")]
            private System.Nullable<bool> @__Published {
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
            
            public virtual System.Nullable<bool> Published {
                get {
                    return @__Published;
                }
                set {
                    @__Published = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v11", OriginalFieldName="o11", Name="Active")]
            private System.Nullable<bool> @__Active {
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
            
            public virtual System.Nullable<bool> Active {
                get {
                    return @__Active;
                }
                set {
                    @__Active = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v12", OriginalFieldName="o12", Name="Rank")]
            private System.Nullable<int> @__Rank {
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
            
            public virtual System.Nullable<int> Rank {
                get {
                    return @__Rank;
                }
                set {
                    @__Rank = value;
                }
            }
        }
    }
}
