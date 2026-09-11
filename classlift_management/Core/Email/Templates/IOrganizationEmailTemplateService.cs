namespace Core.Email.Templates;

public interface IOrganizationEmailTemplateService
{
    TemplatedEmail CourseScheduleCreated(
        OrganizationEmailTemplateContext context,
        string recipientEmail,
        CourseScheduleEmailData data);

    TemplatedEmail CourseSchedulesCreated(
        OrganizationEmailTemplateContext context,
        string recipientEmail,
        CourseScheduleSummaryEmailData data);

    TemplatedEmail CourseScheduleUpdated(
        OrganizationEmailTemplateContext context,
        string recipientEmail,
        CourseScheduleEmailData data);

    TemplatedEmail CourseScheduleDeleted(
        OrganizationEmailTemplateContext context,
        string recipientEmail,
        CourseScheduleEmailData data);

    TemplatedEmail CourseSessionCompleted(
        OrganizationEmailTemplateContext context,
        string recipientEmail,
        CourseSessionCompletedEmailData data);

    TemplatedEmail ScheduleChangeRequested(
        OrganizationEmailTemplateContext context,
        string recipientEmail,
        ScheduleChangeRequestedEmailData data);

    TemplatedEmail CourseConfirmed(
        OrganizationEmailTemplateContext context,
        string recipientEmail,
        CourseConfirmedEmailData data);

    TemplatedEmail ActivityConfirmed(
        OrganizationEmailTemplateContext context,
        string recipientEmail,
        ActivityConfirmedEmailData data);
}
