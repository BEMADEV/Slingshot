﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Slingshot.Core;
using Slingshot.Core.Model;

namespace Slingshot.F1.Utilities.Translators
{
    public static class F1Person
    {
        public static Person Translate( XElement inputPerson, List<FamilyMember> familyMembers, List<PersonAttribute> personAttributes )
        {
            var person = new Person();
            var notes = new List<string>();

            if ( inputPerson.Attribute( "id" ) != null && inputPerson.Attribute( "id" ).Value.AsIntegerOrNull().HasValue )
            {
                person.Id = inputPerson.Attribute( "id" ).Value.AsInteger();

                // names
                person.FirstName = inputPerson.Element( "firstName" ).Value;
                person.NickName = inputPerson.Element( "goesByName" )?.Value;
                person.MiddleName = inputPerson.Element( "middleName" )?.Value;
                person.LastName = inputPerson.Element( "lastName" )?.Value;

                person.Salutation = inputPerson.Element( "salutation" )?.Value;

                var suffix = inputPerson.Element( "suffix" )?.Value;
                if ( suffix.Equals( "Sr.", StringComparison.OrdinalIgnoreCase ) )
                {
                    person.Suffix = "Sr.";
                }
                else if ( suffix.Equals( "Jr.", StringComparison.OrdinalIgnoreCase ) )
                {
                    person.Suffix = "Jr.";
                }
                else if ( suffix.Equals( "Ph.D.", StringComparison.OrdinalIgnoreCase ) )
                {
                    person.Suffix = "Ph.D.";
                }
                else
                {
                    person.Suffix = suffix;
                }

                // communcations (phone & email)
                var communicationsList = inputPerson.Element( "communications" ).Elements( "communication" );
                foreach ( var comm in communicationsList )
                {
                    if ( comm.Element( "communicationType" ).Element( "name" ).Value == "Home Phone" )
                    {
                        person.PhoneNumbers.Add( new PersonPhone
                        {
                            PersonId = person.Id,
                            PhoneType = "Home",
                            PhoneNumber = comm.Element( "communicationValue" ).Value
                        } );
                    }
                    else if ( comm.Element( "communicationType" ).Element( "name" ).Value == "Work Phone" )
                    {
                        person.PhoneNumbers.Add( new PersonPhone
                        {
                            PersonId = person.Id,
                            PhoneType = "Work",
                            PhoneNumber = comm.Element( "communicationValue" ).Value
                        } );
                    }
                    else if ( comm.Element( "communicationType" ).Element( "name" ).Value == "Mobile" )
                    {
                        person.PhoneNumbers.Add( new PersonPhone
                        {
                            PersonId = person.Id,
                            PhoneType = "Mobile",
                            PhoneNumber = comm.Element( "communicationValue" ).Value
                        } );
                    }
                    else if ( comm.Element( "communicationType" ).Element( "name" ).Value == "Email" &&
                              comm.Element( "preferred" ).Value == "true" )
                    {
                        person.Email = comm.Element( "communicationValue" ).Value;
                    }
                }

                // addresses
                var addressList = inputPerson.Element( "addresses" ).Elements( "address" );
                foreach ( var address in addressList )
                {
                    if ( address.Element( "address1" ) != null && address.Element( "address1" ).Value.IsNotNullOrWhitespace() )
                    {
                        var importAddress = new PersonAddress();
                        importAddress.PersonId = person.Id;
                        importAddress.Street1 = address.Element( "address1" ).Value;
                        importAddress.Street2 = address.Element( "address2" )?.Value;
                        importAddress.City = address.Element( "city" ).Value;
                        importAddress.State = address.Element( "stProvince" ).Value;
                        importAddress.PostalCode = address.Element( "postalCode" ).Value;
                        importAddress.Country = address.Element( "country" )?.Value;

                        var addressType = address.Element( "addressType" ).Element( "name" ).Value;

                        switch ( addressType )
                        {
                            case "Primary":
                                importAddress.AddressType = AddressType.Home;
                                break;
                            case "Previous":
                                importAddress.AddressType = AddressType.Previous;
                                break;
                            case "Businiess":
                                importAddress.AddressType = AddressType.Work;
                                break;
                        }

                        // only add the address if we have a valid address
                        if ( importAddress.Street1.IsNotNullOrWhitespace() && importAddress.City.IsNotNullOrWhitespace() && importAddress.PostalCode.IsNotNullOrWhitespace() )
                        {
                            person.Addresses.Add( importAddress );
                        }
                    }
                }

                // gender
                var gender = inputPerson.Element( "gender" )?.Value;

                if ( gender == "Male" )
                {
                    person.Gender = Gender.Male;
                }
                else if ( gender == "Female" )
                {
                    person.Gender = Gender.Female;
                }
                else
                {
                    person.Gender = Gender.Unknown;
                }

                // marital status
                var maritalStatus = inputPerson.Element( "maritalStatus" )?.Value;
                switch ( maritalStatus )
                {
                    case "Married":
                        person.MaritalStatus = MaritalStatus.Married;
                        break;
                    case "Single":
                        person.MaritalStatus = MaritalStatus.Single;
                        break;
                    default:
                        person.MaritalStatus = MaritalStatus.Unknown;
                        if ( maritalStatus.IsNotNullOrWhitespace() )
                        {
                            notes.Add( "maritalStatus: " + maritalStatus );
                        }
                        break;
                }

                // connection status
                var status = inputPerson.Element( "status" ).Element( "name" )?.Value;
                person.ConnectionStatus = status;


                // record status
                if ( status == "Inactive Member" )
                {
                    person.RecordStatus = RecordStatus.Inactive;
                }
                else if ( status == "Inactive" )
                {
                    person.RecordStatus = RecordStatus.Inactive;
                }
                else if ( status == "Deceased" )
                {
                    person.RecordStatus = RecordStatus.Inactive;
                }
                else if ( status == "Dropped" )
                {
                    person.RecordStatus = RecordStatus.Inactive;
                }
                else
                {
                    person.RecordStatus = RecordStatus.Active;
                }

                // dates
                person.Birthdate = inputPerson.Element( "dateOfBirth" )?.Value.AsDateTime();
                person.CreatedDateTime = inputPerson.Element( "createdDate" )?.Value.AsDateTime();
                person.ModifiedDateTime = inputPerson.Element( "lastUpdatedDate" )?.Value.AsDateTime();

                // family
                person.FamilyId = inputPerson.Attribute( "householdID" )?.Value.AsInteger();

                if ( inputPerson.Element( "householdMemberType" ).Element( "name" )?.Value == "Head" ||
                     inputPerson.Element( "householdMemberType" ).Element( "name" )?.Value == "Spouse" )
                {
                    person.FamilyRole = FamilyRole.Adult;
                }
                else if ( inputPerson.Element( "householdMemberType" ).Element( "name" )?.Value == "Child" )
                {
                    person.FamilyRole = FamilyRole.Child;
                }
                else
                {
                    // likely the person is a visitor and should belong to their own family
                    person.FamilyRole = FamilyRole.Child;

                    // generate a new unique family id
                    if ( person.FirstName.IsNotNullOrWhitespace() || person.LastName.IsNotNullOrWhitespace() ||
                         person.MiddleName.IsNotNullOrWhitespace() || person.NickName.IsNotNullOrWhitespace() )
                    {
                        MD5 md5Hasher = MD5.Create();
                        var hashed = md5Hasher.ComputeHash( Encoding.UTF8.GetBytes( person.FirstName + person.NickName + person.MiddleName + person.LastName ) );
                        var familyId = Math.Abs( BitConverter.ToInt32( hashed, 0 ) ); // used abs to ensure positive number
                        if ( familyId > 0 )
                        {
                            person.FamilyId = familyId;
                        }
                    }
                    
                }

                // campus
                Campus campus = new Campus();
                person.Campus = campus;

                // Since family members can have different campuses and Slingshot will set the family campus to the first family
                // member it sees, each family member will be assigned the head of household's campus.
                var headOfHousehold = familyMembers
                        .Where( h => h.HouseholdId == person.FamilyId )
                        .OrderBy( h => h.FamilyRoleId )
                        .FirstOrDefault();

                if ( headOfHousehold != null && headOfHousehold.HouseholdCampusId > 0 )
                {
                    campus.CampusName = headOfHousehold.HouseholdCampusName;
                    campus.CampusId = headOfHousehold.HouseholdCampusId.Value;
                }                        

                // person attributes

                // Note: People from F1 can contain orphaned person attributes that could cause
                //  the slingshot import to fail.  To prevent that, we'll check each attribute
                //  and make sure it exists first before importing.
                //
                // There is also the possibility that someone can have two person attributes with
                //  the same key. 
                var attributes = inputPerson.Element( "attributes" );
                var usedAttributeKeys = new List<string>();

                foreach ( var attribute in attributes.Elements() )
                {
                    if ( personAttributes.Any() )
                    {
                        // Add the comment (if not empty) by matching with the KeyName that was Arbitrarily created in F1Api class.  
                        var commentAttributeKey = attribute.Element( "attributeGroup" ).Element( "attribute" ).Element( "name" ).Value.RemoveSpaces().RemoveSpecialCharacters() + "Comment";
                        string comment = attribute.Element("comment").Value;

                        if (personAttributes.Where(p => commentAttributeKey.Equals(p.Key)).Any() )
                        {
                            usedAttributeKeys.Add(commentAttributeKey);

                            if (usedAttributeKeys.Where(a => commentAttributeKey.Equals(a)).Count() <= 1)
                            {
                                if ( comment.IsNotNullOrWhitespace() )
                                {
                                    person.Attributes.Add( new PersonAttributeValue
                                    {
                                        AttributeKey = commentAttributeKey,
                                        AttributeValue = comment,
                                        PersonId = person.Id
                                    } );
                                }
                                else
                                {
                                    person.Attributes.Add( new PersonAttributeValue
                                    {
                                        AttributeKey = commentAttributeKey,
                                        AttributeValue = "True",
                                        PersonId = person.Id
                                    } );
                                }
                                
                            }
                        }

                        // Add the date (if not null) by matching with the KeyName that was Arbitrarily created in F1Api class. 
                        var dateAttributeKey = attribute.Element("attributeGroup").Element("attribute").Element("name").Value.RemoveSpaces().RemoveSpecialCharacters() + "Date";
                        DateTime? startDate = attribute.Element("startDate")?.Value.AsDateTime();

                        if (personAttributes.Where(p => dateAttributeKey.Equals(p.Key)).Any() && startDate != null)
                        {
                            usedAttributeKeys.Add(dateAttributeKey);

                            if (usedAttributeKeys.Where(a => dateAttributeKey.Equals(a)).Count() <= 1)
                            {
                                person.Attributes.Add(new PersonAttributeValue
                                {
                                    AttributeKey = dateAttributeKey,
                                    AttributeValue = startDate.Value.ToString( "o" ), // save as UTC date format
                                    PersonId = person.Id
                                });
                            }
                        }

                    }
                }

                // person requirements.  Validates the person attributes with what was found in F1.
                var requirements = inputPerson.Element( "peopleRequirements" );
                foreach ( var requirement in requirements.Elements() )
                {
                    string requirementId = requirement.Element( "requirement" ).Attribute( "id" ).Value;

                    var requirementStatus = requirement.Element( "requirementStatus" ).Element( "name" ).Value;
                    var requirementStatusKey = requirementId + "_" + requirement.Element( "requirement" ).Element( "name" ).Value
                                                    .RemoveSpaces().RemoveSpecialCharacters() + "Status";

                    if ( personAttributes.Where( p => requirementStatusKey.Equals( p.Key ) ).Any() )
                    {
                        usedAttributeKeys.Add( requirementStatusKey );

                        if ( usedAttributeKeys.Where( a => requirementStatusKey.Equals( a ) ).Count() <= 1 )
                        {
                            person.Attributes.Add( new PersonAttributeValue
                            {
                                AttributeKey = requirementStatusKey,
                                AttributeValue = requirementStatus,
                                PersonId = person.Id
                            } );
                        }
                    }

                    DateTime? requirementDate = requirement.Element( "requirementDate" ).Value.AsDateTime();
                    var requirementDateKey = requirementId + "_" + requirement.Element( "requirement" ).Element( "name" ).Value
                                                    .RemoveSpaces().RemoveSpecialCharacters() + "Date";

                    if ( requirementDate != null )
                    {
                        if ( personAttributes.Where( p => requirementDateKey.Equals( p.Key ) ).Any() )
                        {
                            usedAttributeKeys.Add( requirementDateKey );

                            if ( usedAttributeKeys.Where( a => requirementDateKey.Equals( a ) ).Count() <= 1 )
                            {
                                person.Attributes.Add( new PersonAttributeValue
                                {
                                    AttributeKey = requirementDateKey,
                                    AttributeValue = requirementDate.Value.ToString( "o" ),
                                    PersonId = person.Id
                                } );
                            }
                        }
                    }
                }

                // person fields

                // occupation
                string occupation = inputPerson.Element( "occupation" ).Element( "name" ).Value;
                if ( occupation.IsNotNullOrWhitespace() )
                {
                    person.Attributes.Add( new PersonAttributeValue
                    {
                        AttributeKey = "Position",
                        AttributeValue = occupation,
                        PersonId = person.Id
                    } );
                }

                // employer
                string employer = inputPerson.Element( "employer" ).Value;
                if ( employer.IsNotNullOrWhitespace() )
                {
                    person.Attributes.Add( new PersonAttributeValue
                    {
                        AttributeKey = "Employer",
                        AttributeValue = employer,
                        PersonId = person.Id
                    } );
                }

                // school
                string school = inputPerson.Element( "school" ).Element( "name" ).Value;
                if ( school.IsNotNullOrWhitespace() )
                {
                    person.Attributes.Add( new PersonAttributeValue
                    {
                        AttributeKey = "School",
                        AttributeValue = school,
                        PersonId = person.Id
                    } );
                }

                // denomination
                string denomination = inputPerson.Element( "denomination" ).Element( "name" ).Value;
                if ( denomination.IsNotNullOrWhitespace() )
                {
                    person.Attributes.Add( new PersonAttributeValue
                    {
                        AttributeKey = "Denomination",
                        AttributeValue = denomination,
                        PersonId = person.Id
                    } );
                }

                // former Church
                string formerChurch = inputPerson.Element( "formerChurch" ).Value;
                if ( formerChurch.IsNotNullOrWhitespace() )
                {
                    person.Attributes.Add( new PersonAttributeValue
                    {
                        AttributeKey = "PreviousChurch",
                        AttributeValue = formerChurch,
                        PersonId = person.Id
                    } );
                }


                // write out person notes
                if ( notes.Count() > 0 )
                {
                    person.Note = string.Join( ",", notes );
                }

            }

            return person;
        }
    }
}
