﻿using System;
using SFA.DAS.EmployerCommitments.Domain.Models.Organisation;

namespace SFA.DAS.EmployerCommitments.Web.ViewModels.Organisation
{
    public class OrganisationViewModelBase : ViewModelBase
    {
        public string OrganisationHashedId { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationReferenceNumber { get; set; }
        public DateTime? OrganisationDateOfInception { get; set; }
        public OrganisationType OrganisationType { get; set; }
        public short? PublicSectorDataSource { get; set; }
        public string OrganisationStatus { get; set; }
        public string Sector { get; set; }
        public string OrganisationAddress { get; set; }
    }
}