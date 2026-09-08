namespace Core.Email.Templates;

public interface IOrganizationEmailTemplateService
{
    TemplatedEmail CourseScheduleCreated(
        OrganizationEmailTemplateContext context,
        CourseScheduleEmailData data);

    TemplatedEmail CourseScheduleUpdated(
        OrganizationEmailTemplateContext context,
        CourseScheduleEmailData data);

    TemplatedEmail CourseScheduleDeleted(
        OrganizationEmailTemplateContext context,
        CourseScheduleEmailData data);

    TemplatedEmail CourseSessionCompleted(
        OrganizationEmailTemplateContext context,
        CourseSessionCompletedEmailData data);

    TemplatedEmail ScheduleChangeRequested(
        OrganizationEmailTemplateContext context,
        ScheduleChangeRequestedEmailData data);

    TemplatedEmail CourseConfirmed(
        OrganizationEmailTemplateContext context,
        CourseConfirmedEmailData data);

    TemplatedEmail ActivityConfirmed(
        OrganizationEmailTemplateContext context,
        ActivityConfirmedEmailData data);
}
