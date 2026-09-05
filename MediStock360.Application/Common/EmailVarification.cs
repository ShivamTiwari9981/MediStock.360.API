

using MediStock360.Application.Common.constaints;

namespace MediStock360.Application.Common
{
    public class EmailVarification
    {
        public long NotificationTemplateId { get; set; }
        public string TemplateCode { get; set; }
        public string TemplateName { get; set; }
        public string NotificationType { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string IsActive { get; set; }

    }
}