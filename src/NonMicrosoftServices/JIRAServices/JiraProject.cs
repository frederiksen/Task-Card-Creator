// This source is subject to the MIT License.
// Please see https://github.com/frederiksen/Task-Card-Creator for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Windows.Controls;
using Atlassian.Jira;
using ReportInterface;
using TaskServerServiceInterface;

namespace JIRAServices
{
    public class JiraProject : ITaskProject
    {
        internal JiraUserControl uc;

        public UserControl CreateUserControl(IEnumerable<IReport> supportedReports, IEnumerable<IReport> allReports)
        {
            uc = new JiraUserControl(supportedReports, allReports);
            return uc;
        }

        public IReport SelectedReport => uc.SelectedReport;

        public IEnumerable<string> WorkItemTypeCollection => new List<string>
        {
            // TODO: Maybe load them form JIRA
            "Story",
            "Epic",
            "Task",
            "Bug"
        };

        public List<ReportItem> WorkItems
        {
            get
            {
                var l = new List<ReportItem>();
                foreach (var issue in uc.SelectedIssues)
                {
                    var ri = new ReportItem
                    {
                        Id = issue.Key.Value,
                        Title = issue.Summary,
                        Type = issue.Type.Name.Replace("Sub", string.Empty),
                        Description = issue.Description.NullAsEmpty().StripTagsRegex(),
                        ParentId = issue.ParentIssueKey
                    };

                    ri.Fields.Add("AffectedVersions", issue.AffectsVersions);
                    ri.Fields.Add("Assignee", issue.Assignee);
                    ri.Fields.Add("Components", issue.Components);
                    ri.Fields.Add("Created", issue.Created);
                    ri.Fields.Add("DueDate", issue.DueDate);
                    ri.Fields.Add("Environment", issue.Environment);
                    ri.Fields.Add("FixVersions", issue.FixVersions);
                    ri.Fields.Add("JiraIdentifier", issue.JiraIdentifier);
                    ri.Fields.Add("Labels", issue.Labels);
                    ri.Fields.Add("Priority", issue.Priority);
                    ri.Fields.Add("Project", issue.Project);
                    ri.Fields.Add("Reporter", issue.Reporter);
                    ri.Fields.Add("Resolution", issue.Resolution);
                    ri.Fields.Add("ResolutionDate", issue.ResolutionDate);
                    ri.Fields.Add("SecutrityLevel", issue.SecurityLevel);
                    ri.Fields.Add("Updated", issue.Updated);
                    ri.Fields.Add("Votes", issue.Votes);

                    // add custom fields
                    foreach (CustomFieldValue field in issue.CustomFields)
                    {
                        ri.Fields.Add(field.Name, field);
                    }

                    l.Add(ri);
                }
                return l;
            }
        }
    }
}