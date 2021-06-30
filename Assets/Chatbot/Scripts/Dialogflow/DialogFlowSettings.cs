using UnityEngine;

namespace Dialogflow {
    [CreateAssetMenu(fileName = "AccessSettings", menuName = "Dialogflow/DialogFlow Settings")]
    public class DialogFlowSettings : ScriptableObject {
        [SerializeField]
        private string projectId = "";
        
        public string ProjectId => projectId;

        [SerializeField]
        private string credentialsFileName = "";

        /// <summary>
        /// The name of the .p12 file that contains the service account credentials.
        /// </summary>
        public string CredentialsFileName => credentialsFileName;

        [SerializeField]
        private string languageCode = "";

        /// <summary>
        /// The language code of requests and responses.
        /// </summary>
        public string LanguageCode => languageCode;
    }
}
