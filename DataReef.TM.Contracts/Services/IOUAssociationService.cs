﻿using DataReef.TM.Models;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IOUAssociationService : IDataService<OUAssociation>
    {
        [OperationContract]
        Task<ICollection<OUAssociation>> SmartList(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string filter = "", string include = "", string exclude = "", string fields = "");

        [OperationContract]
        Task PopulatePersonMayEdit(ICollection<Person> people);

        [OperationContract]
        void SetPersonMayEdit(Person person, OUAssociation association);
    }
}