namespace Bol.Core.Model
{
    public class CitizenshipHashTable
    {
        /// <summary>
        /// National Identity card.
        /// </summary>
        public string IdentityCard { get; set; }
        /// <summary>
        /// Passport.
        /// </summary>
        public string Passport { get; set; }
        /// <summary>
        /// Document that proves ownership of the National Identification Number.
        /// </summary>
        public string ProofOfNin { get; set; }
    }
    
    public class GenericHashTable
    {
        /// <summary>
        /// Driving License card.
        /// </summary>
        public string DrivingLicense { get; set; }
        /// <summary>
        /// Document that proves birth.
        /// </summary>
        public string BirthCertificate { get; set; }
        /// <summary>
        /// Other forms of identification such as Student Id Card, Military Id Card, Lawyer Id Card.
        /// </summary>
        public string OtherIdentity { get; set; }
        /// <summary>
        /// A clear passport style photo of person's face.
        /// </summary>
        public string FacePhoto { get; set; }
        /// <summary>
        /// An audio clip that features the person's voice, stating their name and birthdate.
        /// </summary>
        public string PersonalVoice { get; set; }
        /// <summary>
        /// A telephone number, email, or social network id, owned by the person.
        /// </summary>
        public string ProofOfCommunication { get; set; }
        /// <summary>
        /// A document that proves residence such as Power, Telephone or other Utility Bill.
        /// </summary>
        public string ProofOfResidence { get; set; }
    }
}
