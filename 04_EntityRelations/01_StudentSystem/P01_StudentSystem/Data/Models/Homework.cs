using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
    public class Homework
    {
        public Homework(){ }

        public int HomeworkId { get; set; }
        public string Content { get; set; }
        public ContentType ContentType { get; set; }
        public DateTime SubmissionTime { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }

        public Student Student { get; set; }
        public Course Course { get; set; }
    }

    public enum ContentType
    {
        Application,
        Pdf,
        Zip
    }
}
