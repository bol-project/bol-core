using System;
using Bol.Core.Model;

namespace Bol.Core.Abstractions
{
    public interface ICodeNameService
    {
        /// <summary>
        /// Generates the CodeName of an individual based on personal data.
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        string Generate(NaturalPerson person);
        /// <summary>
        /// Generates the CodeName of an organization based on incorporation data.
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        string Generate(Company company);
        /// <summary>
        /// Generates the ShortHash part of an Individual's CodeName.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="birthDate"></param>
        /// <param name="nin"></param>
        /// <returns></returns>
        string GenerateShortHash(string firstName, DateTime birthDate, string nin);
        /// <summary>
        /// Generates the ShortHash part of an Organization's CodeName.
        /// </summary>
        /// <param name="firstWord"></param>
        /// <param name="secondWord"></param>
        /// <param name="incorporationDate"></param>
        /// <param name="vatNumber"></param>
        /// <returns></returns>
        string GenerateShortHash(string firstWord, string secondWord, DateTime incorporationDate, string vatNumber);
        /// <summary>
        /// Adds checksum suffix on a CodeName.
        /// </summary>
        /// <param name="codeName"></param>
        /// <returns></returns>
        string AddCodeNameChecksum(string codeName);
    }
}
