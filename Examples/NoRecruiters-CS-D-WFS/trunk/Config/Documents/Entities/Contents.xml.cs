#pragma warning disable 0169, 0649, 0618
// -------------------------------------------864187d2-c0eb-4620-9c81-c789026db391
//
//		Generated document definition classes
//		Workflow Server code generator v.4.0.1.0
//		All modifications to the content of this file will be lost
//		when file will be overwritten by the code generator
//
//		Generated from the Xml schema located at 
//			Config\Documents\Entities\Contents.xml
//			as of 6/22/2009 2:15 
//
// -------------------------------------------------------------------------------
namespace NoRecruiters3 {
    using WorkflowServer.Foundation.Documents;
    using WorkflowServer.Foundation.Documents.MetaInfo;
    
    
    /// WS_documentDependency WorkflowServer.Foundation.Documents.Document=Unknown
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="Contents", IsReference=true)]
    [DocumentVersion("d6a879c36844fba838428e59bd4cff3f4ae9ce1")]
    public partial class Contents : WorkflowServer.Foundation.Documents.Document {
        
        // Contains names of metanodes. Used at run time by the ElementMetaNode.Build method
        static string[] WS_metanode_names = new string[] {
                "docID",
                "ContentsText"};
        
        // Local metanode index. Populated runtime by the ElmentMetaNode.Build method
        static WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode[] WS_metanodes;
        
        public static string WS_documentSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns:wdf=""http://schemas.hill30.com/workflow/documents"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element wdf:read=""s_contents"" wdf:save=""iu_contents"" wdf:delete=""d_contents"" name=""document"">
    <xs:complexType>
      <xs:attribute wdf:param-name=""ContentsId"" name=""docID"" type=""xs:ID"" />
      <xs:attribute wdf:param-name=""Contents"" name=""ContentsText"" type=""xs:string"" />
    </xs:complexType>
  </xs:element>
</xs:schema>";
        
        // Private field for the Value value of the the ContentsText document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="v1", Order=2001, IsRequired=false)]
        private string v1;
        
        // Private field for the Original value of the the ContentsText document field
        [System.Runtime.Serialization.DataMemberAttribute(Name="o1", Order=3001, IsRequired=false)]
        private string o1;
        
        public Contents(WorkflowServer.Foundation.Documents.DocumentFactory factory) : 
                base(factory) {
            return;
        }
        
        public Contents(WorkflowServer.Foundation.Documents.DocumentFactory factory, System.Xml.XmlReader content) : 
                base(factory, content) {
            return;
        }
        
        [WorkflowServer.Foundation.Documents.DocumentFieldAttribute(ValueFieldName="v1", OriginalFieldName="o1", Name="ContentsText")]
        private string @__ContentsText {
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
        
        public virtual string ContentsText {
            get {
                return @__ContentsText;
            }
            set {
                @__ContentsText = value;
            }
        }
        
        public static WorkflowServer.Foundation.Documents.Factories.StoredProcedure.Factory<Contents> DefaultFactory {
            get {
                return ((WorkflowServer.Foundation.Documents.Factories.StoredProcedure.Factory<Contents>)(WorkflowServer.Foundation.Documents.DocumentFactory.GetFactoryByName("Contents")));
            }
        }
    }
}
