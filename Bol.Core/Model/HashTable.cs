using YamlDotNet.Serialization;

namespace Bol.Core.Model
{
    public class CitizenshipHashTable
    {
        /// <summary>
        /// National Identity card.
        /// </summary>
        [YamlMember(Order = 0)]
        public string IdentityCard { get; set; }
        /// <summary>
        /// Passport.
        /// </summary>
        [YamlMember(Order = 1)]
        public string Passport { get; set; }
        /// <summary>
        /// Document that proves ownership of the National Identification Number.
        /// </summary>
        [YamlMember(Order = 2)]
        public string ProofOfNin { get; set; }
    }
    
    public class GenericHashTable : CitizenshipHashTable
    {
        /// <summary>
        /// Driving License card.
        /// </summary>
        [YamlMember(Order = 3)]
        public string DrivingLicense { get; set; }
        /// <summary>
        /// Document that proves birth.
        /// </summary>
        [YamlMember(Order = 4)]
        public string BirthCertificate { get; set; }
        /// <summary>
        /// Other forms of identification such as Student Id Card, Military Id Card, Lawyer Id Card.
        /// </summary>
        [YamlMember(Order = 5)]
        public string OtherIdentity { get; set; }
        /// <summary>
        /// A clear passport style photo of person's face.
        /// </summary>
        [YamlMember(Order = 6)]
        public string FacePhoto { get; set; }
        /// <summary>
        /// An audio clip that features the person's voice, stating their name and birthdate.
        /// </summary>
        [YamlMember(Order = 7)]
        public string PersonalVoice { get; set; }
        /// <summary>
        /// A telephone number, email, or social network id, owned by the person.
        /// </summary>
        [YamlMember(Order = 8)]
        public string ProofOfCommunication { get; set; }
        /// <summary>
        /// A document that proves residence such as Power, Telephone or other Utility Bill.
        /// </summary>
        [YamlMember(Order = 9)]
        public string ProofOfResidence { get; set; }
    }
}
