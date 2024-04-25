
namespace LMS.Application.Common
{
    public class ResponseMessageConstants
    {
        public const string InvalidEmail = "Email is not valid. - UserEmailId - {0} ";

        public const string InvalidUser = "User not found in the db. EmailId - {0}";
        public const string InvalidCourse = "Course not found in the db. Id - {0}";
        public const string InvalidLearningPath = "Learning path not found in the db. Title - {0}";

        public const string CourseAlreadyEnrolled = "Course Already enrolled by the student";


        public const string UserAlreadyExist = "User already exists in the db. EmailId - {0}";
        public const string UsernameAlreadyExist = "Username already exists. please try again. EmailId - {0}";

        public const string UserCreationFailed = "User creation failed. EmailId - {0}";
        public const string UserCreationSuccess = "Successfully created new user. EmailId - {0}";

        public const string MessagePostingFailed = "Message posting failed. message - {0}";
        public const string MessagePostingSuccess = "Successfully posted new message. message - {0}";

        public const string LoginFailed = "User Login failed. email - {0}";
        public const string LoginSuccess = "User login success. email - {0}";

        public const string UserUpdationFailed = "User updating failed. email - {0}";
        public const string UserUpdationSuccess = "User updating success. email - {0}";

        public const string UserDeletionFailed = "User deletion failed. email - {0}";
        public const string UserDeletionSuccess = "User deletion success. email - {0}";

        public const string UnprivilegedUser = "Logged in user does not have sufficient permission to perform this action. email - {0}";

        public const string CourseAlreadyExist = "Course already exists in the db. EmailId - {0}";
        public const string LearningPathAlreadyExist = "Learning path already exists in the db. id - {0}";

        public const string CourseCreationFailed = "Course creation failed. Title - {0}";
        public const string CourseCreationSuccess = "Successfully created new course. Title - {0}";

        public const string LearningPathCreationFailed = "Learning Path creation failed. Title - {0}";
        public const string LearningPathCreationSuccess = "Successfully created new learning path. Title - {0}";

        public const string CourseDeletionFailed = "Course deletion failed. Id - {0}";
        public const string CourseDeletionSuccess = "Course deletion success. Id - {0}";

        public const string LearningPathDeletionFailed = "LearningPath deletion failed. id - {0}";
        public const string LearningPathDeletionSuccess = "LearningPath deletion success. id - {0}";

        public const string CourseEnrollementFailed = "Course Enrollement Failed.";
        public const string CourseEnrollmentSuccess = "Course Enrollement success.";



    }
}
