using DataReef.Core.Attributes;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DTOs.Properties;
using DataReef.TM.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace DataReef.TM.Models
{
    [DataContract(IsReference = true)]
    [Versioned]
    public class PropertyNote : EntityBase
    {

        public PropertyNote()
        {
            
        }

        #region Properties

        [DataMember]
        public Guid PersonID { get; set; }

        [DataMember]
        public Guid PropertyID { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public Guid ParentID { get; set; }

        [DataMember]
        public string ContentType { get; set; }
        
        #endregion

        #region Navigation

        [DataMember]
        [ForeignKey("PersonID")]
        public Person Person { get; set; }

        [DataMember]
        [ForeignKey("PropertyID")]
        public Property Property { get; set; }

        #endregion

        #region Computed Properties

        [NotMapped]
        public IEnumerable<PropertyNoteTagDTO> ContentTags
        {
            get
            {
                var returnList = new List<PropertyNoteTagDTO>();
                if (string.IsNullOrEmpty(Content))
                {
                    return returnList;
                }

                var emailTagPattern = @"\[email:'(.*?)'\](.*?)\[\/email]";
                var regex = new Regex(emailTagPattern);

                var matches = regex.Matches(Content);

                if (matches != null)
                {
                    foreach (Match m in matches)
                    {
                        if (m.Groups.Count == 3)
                        {
                            returnList.Add(new PropertyNoteTagDTO
                            {
                                Name = m.Groups[2].Value,
                                Email = m.Groups[1].Value,
                                Tag = m.Value

                            });
                        }
                    }
                }

                return returnList;
            }
        }
        #endregion
    }
}