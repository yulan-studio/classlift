using Core.Models;

namespace Core.Email.Templates;

public sealed record OrganizationEmailTemplateContext(
    string OrganizationName,
    string ReplyToEmail,
    Uri TrustedPortalBaseUri,
    OrganizationTerminology Terminology);
