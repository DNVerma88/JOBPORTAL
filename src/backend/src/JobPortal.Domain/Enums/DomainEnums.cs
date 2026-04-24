namespace JobPortal.Domain.Enums;

public enum ApplicationStatus
{
    Pending = 1,
    Screening = 2,
    ShortListed = 3,
    InterviewScheduled = 4,
    Interviewed = 5,
    OfferSent = 6,
    Hired = 7,
    Rejected = 8,
    Withdrawn = 9,
    OnHold = 10
}

public enum JobPostingStatus
{
    Draft = 1,
    Published = 2,
    Paused = 3,
    Closed = 4,
    Expired = 5,
    Archived = 6
}

public enum JobType
{
    FullTime = 1,
    PartTime = 2,
    Contract = 3,
    Freelance = 4,
    Internship = 5,
    Temporary = 6
}

public enum WorkMode
{
    OnSite = 1,
    Remote = 2,
    Hybrid = 3
}

public enum ExperienceLevel
{
    EntryLevel = 1,
    MidLevel = 2,
    Senior = 3,
    Lead = 4,
    Manager = 5,
    Executive = 6
}

public enum NotificationChannel
{
    InApp = 1,
    Email = 2,
    SMS = 3,
    Push = 4
}

public enum NotificationType
{
    JobAlert = 1,
    ApplicationStatus = 2,
    Message = 3,
    InterviewInvite = 4,
    OfferLetter = 5,
    SystemAlert = 6,
    Announcement = 7
}

public enum NotificationDeliveryScope
{
    Public = 1,
    Tenant = 2,
    Private = 3,
    Role = 4
}

public enum InterviewType
{
    Phone = 1,
    Video = 2,
    InPerson = 3,
    Technical = 4,
    HR = 5
}

public enum SubscriptionTier
{
    Free = 1,
    Basic = 2,
    Professional = 3,
    Enterprise = 4
}

public enum Gender
{
    Male = 1,
    Female = 2,
    NonBinary = 3,
    PreferNotToSay = 4
}

public enum ProfileVisibility
{
    Public = 1,
    Private = 2,
    RecruiterOnly = 3
}

public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Refunded = 4,
    Cancelled = 5
}

public enum OfferLetterStatus
{
    Sent = 1,
    Accepted = 2,
    Declined = 3,
    Expired = 4,
    Revoked = 5
}

public enum AlertFrequency
{
    Instant = 1,
    Daily = 2,
    Weekly = 3
}
