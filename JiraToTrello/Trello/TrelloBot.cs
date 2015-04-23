using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JiraToTrello.JIRA;
using TechTalk.JiraRestClient;
using TrelloNet;
using TrelloNet.Extensions;

namespace JiraToTrello.Trello
{
    class TrelloBot
    {
        private const string TrackingString = "Issue is now being tracked.";
        private const string TextSplit = "---SplittingSomeText---";

        private readonly ITrello _trello;
        private IList<Card> _assignedCards;
        private readonly IList<string> _trackedIssues;
        private readonly Regex _projectRegex;

        public TrelloBot(ITrello trello, string projectToTrack)
        {
            this._trello = trello;
            _trackedIssues = new List<string>();
            _projectRegex = new Regex(String.Format(@"\b{0}-\d+", projectToTrack));

            SetAssignedCards();
        }

        public IList<Card> GetAssignedCards()
        {
            SetAssignedCards();

            return _assignedCards;
        }

        private void SetAssignedCards()
        {
            _assignedCards = _trello.Cards.ForMe().ToList();

            foreach (var card in _assignedCards)
            {
                //Check every card we're assigned to. If we haven't commented, its a new one
                IEnumerable<string> comments = GetComments(card);

                if (!comments.Any())
                {
                    //We've found a card we haven't started tracking yet
                    _trello.Cards.AddComment(card, TrackingString);
                }
            }
        }

        private IEnumerable<string> GetComments(Card card)
        {
            IEnumerable<string> comments = _trello.Actions.AutoPaged(Paging.MaxLimit)
                .ForCard(card, new[] { ActionType.CommentCard })
                .OfType<CommentCardAction>()
                .Select(x => x.Data.Text);
            return comments;
        }

        public IList<string> GetIssuesToTrack()
        {
            if (_trackedIssues.Count == _assignedCards.Count())
                return _trackedIssues;

            foreach (var card in _assignedCards)
            {
                var title = card.Name;
                if (_projectRegex.IsMatch(title))
                {
                    //We have a match - it contains our JIRA project identifier followed by a dash and numbers
                    //Extract JUST the pattern - in case the user has text following it
                    var match = _projectRegex.Match(title);
                    _trackedIssues.Add(match.Value);
                }
            }

            return _trackedIssues;
        }

        public void ProcessIssues<T>(IList<Issue<T>> issues) where T : IssueFields, new()
        {
            
            Type customType = typeof (T);
            var attributes = new Dictionary<PropertyInfo, FieldDataAttribute>();
            string trelloString = String.Empty;
            string newDesc = String.Empty;

            //Grab all the Field metadata for each custom field
            foreach (var field in customType.GetProperties())
            {
                foreach (var attr in field.GetCustomAttributes(false))
                {
                    var isThisFieldData = attr as FieldDataAttribute;
                    if(isThisFieldData != null)
                    {
                        attributes.Add(field, isThisFieldData);
                    }
                }
            }

            foreach (var issue in issues)
            {
                try
                {
                    var matchedCard = _assignedCards.Single(x => x.Name.Contains(issue.key));
                    var comments = GetComments(matchedCard);
                    var enumeratedComments = comments as IList<string> ?? comments.ToList();
                    var customFields = customType.GetProperties();

                    foreach (var customField in customFields)
                    {
                        if (attributes.ContainsKey(customField))
                        {
                            var fieldData = attributes[customField];
                            trelloString = ConstructTrelloString(customField.GetValue(issue.fields, null), fieldData);
                            if (fieldData.FieldType == CustomFieldType.Description)
                            {
                                newDesc += trelloString;
                            }
                            else if (fieldData.FieldType == CustomFieldType.Comment)
                            {
                                
                                var splitComments = trelloString.Split(new String[] {TextSplit}, StringSplitOptions.None);
                                foreach (var splitComment in splitComments)
                                {
                                    if (splitComment != String.Empty && !enumeratedComments.Contains(splitComment))
                                    {
                                        _trello.Cards.AddComment(matchedCard, splitComment);
                                    }
                                }
                            }
                        }
                    }

                    newDesc.Replace(TextSplit, String.Empty);
                    if (matchedCard.Desc != newDesc)
                                    _trello.Cards.ChangeDescription(matchedCard, newDesc);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error! Unable to find the correct card/card information! " + e.Message);
                }
            }

            
        }

        private string ConstructTrelloString(object obj, FieldDataAttribute attr)
        {
            //Get subfields of this object. Continue getting subfields till we get a primitive/string
            string ret = String.Empty;

            //Have we found a primitive? Return it if so
            if (obj is string || obj.GetType().IsPrimitive)
            {
                return String.Format("**{1}**:{0} {2}{0}{0}", Environment.NewLine, attr.FieldName,
                                       obj.ToString()); 
            }

            
            //TODO: This if and else could use the same method within
            //Is this an enumerable object?
            var enumObj = obj as IEnumerable;
            if (enumObj != null)
            {
                foreach (var singleObj in enumObj)
                {
                    foreach (var field in singleObj.GetType().GetProperties())
                    {
                        if (attr.FieldName != String.Empty)
                        {
                            ret += String.Format("{1}{0}---{0}", Environment.NewLine, attr.FieldName);
                        }
                        //Insert some text to split here - if these are comments, we want to add them seperately
                        foreach (var custAttr in field.GetCustomAttributes(false))
                        {
                            var isThisFieldData = custAttr as FieldDataAttribute;
                            if (isThisFieldData != null)
                            {
                                //Is there more? Recurse
                                ret += ConstructTrelloString(field.GetValue(singleObj), isThisFieldData);

                            }
                        }
                    }
                    ret += TextSplit;
                }
            }
            else
            {
                foreach (var field in obj.GetType().GetProperties())
                {
                    if (attr.FieldName != String.Empty)
                    {
                        ret += String.Format("{1}{0}---{0}", Environment.NewLine, attr.FieldName);
                    }
                    //Get this new fields FieldDataAttribute to pass down
                    foreach (var custAttr in field.GetCustomAttributes(false))
                    {
                        var isThisFieldData = custAttr as FieldDataAttribute;
                        if (isThisFieldData != null)
                        {
                            //Is there more? Recurse
                            ret += ConstructTrelloString(field.GetValue(obj), isThisFieldData);
                        }
                    }
                }
            }

            return ret;

        }

        public void ProcessIssues(IList<Issue> issues)
        {
            foreach (var issue in issues)
            {
                var matchedCard = _assignedCards.Single(x => x.Name.Contains(issue.key));

                if(matchedCard.Desc != issue.fields.description)
                    _trello.Cards.ChangeDescription(matchedCard, issue.fields.description);

                var comments = GetComments(matchedCard);
                var enumeratedComments = comments as IList<string> ?? comments.ToList();
                foreach (var jiraComment in issue.fields.comments)
                {
                    if (!enumeratedComments.Contains(jiraComment.body))
                    {
                        _trello.Cards.AddComment(matchedCard, jiraComment.body);
                    }
                }
            }
        }


    }
}
