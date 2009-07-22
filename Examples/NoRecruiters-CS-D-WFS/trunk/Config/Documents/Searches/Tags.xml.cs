#pragma warning disable 0169, 0649, 0618
// -------------------------------------------864187d2-c0eb-4620-9c81-c789026db391
//
//		Generated document definition classes
//		Workflow Server code generator v.4.0.1.0
//		All modifications to the content of this file will be lost
//		when file will be overwritten by the code generator
//
//		Generated from the Xml schema located at 
//			Config\Documents\Searches\Tags.xml
//			as of 6/22/2009 2:15 
//
// -------------------------------------------------------------------------------
namespace NoRecruiters3 {
    using WorkflowServer.Foundation.Documents;
    using WorkflowServer.Foundation.Documents.MetaInfo;
    
    
    /// WS_documentDependency WorkflowServer.Foundation.Documents.Document=Unknown
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="Tags", IsReference=true)]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(data_NodeCollection))]
    [DocumentVersion("313ebd413adc15422f3b68cf6163bb96ea72c8a")]
    public partial class Tags : WorkflowServer.Foundation.Documents.Document {
        
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
        <xs:element wdf:read=""s_tag_list(count)"" maxOccurs=""unbounded"" name=""data"">
          <xs:complexType>
            <xs:attribute name=""TagText"" type=""xs:string"" />
            <xs:attribute name=""SafeText"" type=""xs:string"" />
            <xs:attribute wdf:param-name=""Cnt"" name=""Count"" type=""xs:int"" />
          </xs:complexType>
        </xs:element>
      </xs:sequence>
      <xs:attribute wdf:transient=""true"" name=""docID"" type=""xs:ID"" />
    </xs:complexType>
  </xs:element>
</xs:schema>";
        
        // Private field for the Value value of the the data document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v1", Order=2001, IsRequired=false)]
        private NoRecruiters3.Tags.data_NodeCollection v1;
        
        public Tags(WorkflowServer.Foundation.Documents.DocumentFactory factory) : 
                base(factory) {
            return;
        }
        
        public Tags(WorkflowServer.Foundation.Documents.DocumentFactory factory, System.Xml.XmlReader content) : 
                base(factory, content) {
            return;
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(FieldType=typeof(NoRecruiters3.Tags.data_Node), ValueFieldName="v1")]
        public virtual NoRecruiters3.Tags.data_NodeCollection data {
            get {
                return v1;
            }
        }
        
        public static WorkflowServer.Foundation.Documents.Factories.StoredProcedure.Factory<Tags> DefaultFactory {
            get {
                return ((WorkflowServer.Foundation.Documents.Factories.StoredProcedure.Factory<Tags>)(WorkflowServer.Foundation.Documents.DocumentFactory.GetFactoryByName("Tags")));
            }
        }
        
        [System.SerializableAttribute()]
        [System.Runtime.Serialization.DataContractAttribute(Name="data_NodeCollection", IsReference=true)]
        [System.Runtime.Serialization.KnownTypeAttribute(typeof(NoRecruiters3.Tags.data_Node))]
        public partial class data_NodeCollection : WorkflowServer.Foundation.Documents.DocumentNodeCollection {
            
            public data_NodeCollection(WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode metaNode, WorkflowServer.Foundation.Documents.DocumentNode parent) : 
                    base(metaNode, parent) {
            }
            
            public virtual NoRecruiters3.Tags.data_Node AppendNode() {
                return ((NoRecruiters3.Tags.data_Node)(base.AppendNode()));
            }
            
            public virtual NoRecruiters3.Tags.data_Node InsertNodeBefore(NoRecruiters3.Tags.data_Node sibling) {
                return ((NoRecruiters3.Tags.data_Node)(base.InsertNodeBefore(sibling)));
            }
        }
        
        [System.SerializableAttribute()]
        [System.Runtime.Serialization.DataContractAttribute(Name="data_Node", IsReference=true)]
        public partial class data_Node : WorkflowServer.Foundation.Documents.DocumentNode {
            
            // Contains names of metanodes. Used at run time by the ElementMetaNode.Build method
            static string[] WS_metanode_names = new string[] {
                    "TagText",
                    "SafeText",
                    "Count"};
            
            // Local metanode index. Populated runtime by the ElmentMetaNode.Build method
            static WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode[] WS_metanodes;
            
            // Private field for the Value value of the the TagText document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v0", Order=2000, IsRequired=false)]
            private string v0;
            
            // Private field for the Original value of the the TagText document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o0", Order=3000, IsRequired=false)]
            private string o0;
            
            // Private field for the Value value of the the SafeText document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v1", Order=2001, IsRequired=false)]
            private string v1;
            
            // Private field for the Original value of the the SafeText document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o1", Order=3001, IsRequired=false)]
            private string o1;
            
            // Private field for the Value value of the the Count document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="v2", Order=2002, IsRequired=false)]
            private System.Nullable<int> v2;
            
            // Private field for the Original value of the the Count document field
            [System.Runtime.Serialization.DataMemberAttribute(Name="o2", Order=3002, IsRequired=false)]
            private System.Nullable<int> o2;
            
            public data_Node(WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode metaNode, WorkflowServer.Foundation.Documents.DocumentNodeCollection collection, WorkflowServer.Foundation.Documents.DocumentNode sibling, WorkflowServer.Foundation.Documents.DocumentNode.InitMode initFor) : 
                    base(metaNode, collection, sibling, initFor) {
                return;
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v0", OriginalFieldName="o0", Name="TagText")]
            private string @__TagText {
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
            
            public virtual string TagText {
                get {
                    return @__TagText;
                }
                set {
                    @__TagText = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v1", OriginalFieldName="o1", Name="SafeText")]
            private string @__SafeText {
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
            
            public virtual string SafeText {
                get {
                    return @__SafeText;
                }
                set {
                    @__SafeText = value;
                }
            }
            
            [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v2", OriginalFieldName="o2", Name="Count")]
            private System.Nullable<int> @__Count {
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
            
            public virtual System.Nullable<int> Count {
                get {
                    return @__Count;
                }
                set {
                    @__Count = value;
                }
            }
        }
    }
}
