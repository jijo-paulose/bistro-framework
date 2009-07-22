#pragma warning disable 0169, 0649, 0618
// -------------------------------------------864187d2-c0eb-4620-9c81-c789026db391
//
//		Generated document definition classes
//		Workflow Server code generator v.4.0.1.0
//		All modifications to the content of this file will be lost
//		when file will be overwritten by the code generator
//
//		Generated from the Xml schema located at 
//			Config\Documents\System\message-queue.xml
//			as of 6/22/2009 2:15 
//
// -------------------------------------------------------------------------------
namespace NoRecruiters3 {
    using WorkflowServer.Foundation.Documents;
    using WorkflowServer.Foundation.Documents.MetaInfo;
    
    
    /// WS_documentDependency WorkflowServer.Foundation.Documents.DocumentMessageQueue=Unknown
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name="message_queue", IsReference=true)]
    [DocumentVersion("128b2253bcda5fa28e5f5e50acee812c38568289")]
    public partial class message_queue : WorkflowServer.Foundation.Documents.DocumentMessageQueue {
        
        // Contains names of metanodes. Used at run time by the ElementMetaNode.Build method
        static string[] WS_metanode_names = new string[] {
                "docID",
                "message"};
        
        // Local metanode index. Populated runtime by the ElmentMetaNode.Build method
        static WorkflowServer.Foundation.Documents.MetaInfo.DocumentMetaNode[] WS_metanodes;
        
        public message_queue(WorkflowServer.Foundation.Documents.DocumentFactory factory, System.Xml.XmlReader content) : 
                base(factory, content) {
            return;
        }
        
        public message_queue(WorkflowServer.Foundation.Documents.DocumentFactory factory) : 
                base(factory) {
            return;
        }
        
        public static WorkflowServer.Foundation.Documents.FactoryEnvelope<message_queue> DefaultFactory {
            get {
                return ((WorkflowServer.Foundation.Documents.FactoryEnvelope<message_queue>)(WorkflowServer.Foundation.Documents.DocumentFactory.GetFactoryByName("message-queue")));
            }
        }
    }
}
