﻿using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DataViews.Inquiries;
using DataReef.TM.Models.DataViews.OnBoarding;
using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.DTOs.OUs;
using DataReef.TM.Models.DTOs.Persons;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Finance;
using DataReef.TM.Models.PubSubMessaging;
using DataReef.TM.Models.Reporting.Settings;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IOUService : IDataService<OU>
    {
        [OperationContract]
        ICollection<OU> ListRootsForPerson(Guid personID, string include, string exclude, string fields, string query = null);

        [OperationContract]
        ICollection<Guid> ListRootGuidsForPerson(Guid personID);

        [OperationContract]
        ICollection<OU> ListAllForPerson(Guid personID);

        [OperationContract]
        ICollection<OU> ListAllForPersonAndSetting(Guid personID, string settingName);

        [OperationContract]
        ICollection<Person> GetMembers(OUMembersRequest request);

        [OperationContract]
        OU OUBuilder(OU ou, string include = "", string exclude = "", string fields = "", bool ancestors = false, bool includeDeleted = false);

        [OperationContract]
        ICollection<OUAssociation> PopulateAssociationsOUs(ICollection<OUAssociation> associations);

        [OperationContract]
        ICollection<InquiryStatisticsForOrganization> GetInquiryStatisticsForOrganization(Guid ouId, OUReportingSettings reportSettings, DateTime? specifiedDay = null, DateTime? StartRangeDay = null, DateTime? EndRangeDay = null, IEnumerable<Guid> excludedReps = null);

        [OperationContract]
        ICollection<InquiryStatisticsForPerson> GetInquiryStatisticsForSalesPeople(Guid ouId, OUReportingSettings reportSettings, DateTime? specifiedDay = null, DateTime? StartRangeDay = null, DateTime? EndRangeDay = null, IEnumerable<Guid> excludedReps = null);

        [OperationContract]
        ICollection<Guid> ConditionalGetActiveUserIDsForCurrentAndSubOUs(Guid ouID, bool deepSearch);

        [OperationContract]
        ICollection<Guid> ConditionalGetActivePeopleIDsForCurrentAndSubOUs(Guid ouID, bool deepSearch);

        [OperationContract]
        ICollection<Guid> ConditionalGetActiveOUAssociationIDsForCurrentAndSubOUs(Guid ouID, bool deepSearch);

        [OperationContract]
        ICollection<Territory> GetOUTreeTerritories(Guid ouID, string territoryName, bool deletedItems);

        [OperationContract]
        ICollection<WebHook> GetWebHooks(Guid ouID, EventDomain domainFlags);


        [OperationContract]
        ICollection<Guid> GetHierarchicalOrganizationGuids(ICollection<Guid> parentOUs, ICollection<Guid> excludeOUs = null);

        [OperationContract]
        List<GuidNamePair> GetAllSubOUIdsAndNamesOfSpecifiedOus(string ouIDs);

        [OperationContract]
        ICollection<Guid> GetOUAndChildrenGuids(Guid ouid);

        [OperationContract]
        void MoveOU(Guid ouID, Guid newParentOUID);

        [OperationContract]
        float GetTokenPriceInDollars(Guid ouid);

        [OperationContract]
        ICollection<FinancePlanDefinition> GetFinancePlanDefinitions(Guid ouid, string include = "", string exclude = "", string fields = "");

        [OperationContract]
        ICollection<FinancePlanDefinition> GetFinancePlanDefinitionsProposal(Guid proposalid, string include = "", string exclude = "", string fields = "");

        [OperationContract]
        OU GetByShapesVersion(Guid ouid, ICollection<OuShapeVersion> ouShapeVersions, bool deletedItems = false, string include = "");

        [OperationContract]
        LookupDataView GetOnboardingLookupData(Guid? parentId);

        [OperationContract]
        void CreateNewOU(OnboardingOUDataView req);

        [OperationContract]
        void EditOU(Guid ouid, OnboardingOUDataView req);

        [OperationContract]
        void EditOUSettings(Guid ouID, OnboardingOUSettingsDataView req);

        [OperationContract]
        void AddOUSettingsTest();

        [OperationContract]
        void AddGenericProposalOUSettings();

        [OperationContract]
        OU GetWithAncestors(Guid uniqueId, string include = "", string exclude = "", string fields = "", bool summary = true, string query = "", bool deletedItems = false);

        [OperationContract]
        List<OUsAndRoleTree> GetOUsRoleTree(Guid personID);

        [OperationContract]
        bool ProcessEvent(EventMessage eventMessage);

        [OperationContract]
        List<OUWithAncestors> GetSubOUsForOuSetting(Guid parentOUID, string settingName);

        [OperationContract]
        Tuple<string, List<ActiveUserDTO>> GetActiveUsersForOrgID(Guid ouid);

        [OperationContract]
        OUActiveUsersCSV GetActiveUsersCSV(Guid ouid);

        [OperationContract]
        SBOUDTO GetSmartboardOus(Guid ouID, string apiKey);

        [OperationContract]
        IEnumerable<SBOUDTO> GetSmartboardAllOus(string apiKey);

        [OperationContract]
        OUChildrenAndTerritories GetOUWithChildrenAnTerritories(Guid ouID);

        [OperationContract]
        List<GuidNamePair> GetAncestorsForOU(Guid ouID);

        [OperationContract]
        IEnumerable<Person> GetPersonsAssociatedWithOUOrAncestor(Guid ouID, string name, string email);

        [OperationContract]
        IEnumerable<SBOURoleDTO> GetAllRoles(string apiKey);

        [OperationContract]
        IEnumerable<zapierOus> GetzapierOusList(float? Lat, float? Lon, string apiKey );

        [OperationContract]
        IEnumerable<Territories> GetTerritoriesListByOu(float? Lat, float? Lon, Guid ouid);

        [OperationContract]
        string GetApikeyByOU(Guid ouid);

       
        [OperationContract]
        IEnumerable<SBOU> GetOusList(string name);

        [OperationContract]
        string InsertApikeyForOU(SBOUID request, string apikey);

        [OperationContract]
        IEnumerable<OURole> GetOuRoles();

        [OperationContract]
        void UpdateOuRoles(List<OURole> roles);

        [OperationContract]
        bool UpdateOuRolesPermission(List<OURole> roles);

        [OperationContract]
        bool UpdateOuPermissions(List<OU> ous);

        [OperationContract]
        OU InheritsParentOuPermissions(OU ou);

        [OperationContract]
        OURole GetOuRoleByID(Guid? ouid);

        [OperationContract]
        void CreateNewOURole(OURole req);

        [OperationContract]
        IEnumerable<GuidNamePair> SBGetOuRoles();

        [OperationContract]
        void EditOURole(Guid ouid, OURole req);

        [OperationContract]
        FavouriteOu InsertFavouriteOu(Guid ouId, Guid personID);

        [OperationContract]
        void RemoveFavouriteOu(Guid ouId, Guid personID);

        [OperationContract]
        List<OU> FavouriteOusList(Guid personID, bool deletedItems = false);

        [OperationContract]
        List<Territory> FavouriteTerritoriesList(Guid personID, bool deletedItems = false, string include = "Assignments.Person,Prescreens"); 

        [OperationContract]
        string InsertMasterTerritory();
    }
}