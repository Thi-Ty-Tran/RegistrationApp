//    Name: Thi Ty Tran
//    Date Created: Nov 28, 2024
//    Description: DC Registration App - Assignment 5
//    Last modified: Dec 5, 2024 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace RegistrationApp
{
    /// <summary>
    ///  Represents a student in the registration system
    /// </summary>
    public class Student
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string sin { get; set; }
        public string email { get; set; }
        public string highSchoolGrade { get; set; }
        public string admissionTestScore { get; set; }
        public string campusLocation { get; set; }
        public string program { get; set; }
    }
}
