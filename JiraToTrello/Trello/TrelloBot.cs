using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrelloNet;
using TrelloNet.Extensions;

namespace JiraToTrello.Trello
{
    class TrelloBot
    {
        private const string TrackingString = "Issue is now being tracked.";      

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
                IEnumerable<string> comments = _trello.Actions.AutoPaged(Paging.MaxLimit)
                    .ForCard(card, new[] {ActionType.CommentCard})
                    .OfType<CommentCardAction>()
                    .Where(y => y.Data.Text.Contains(TrackingString))
                    .Select(x => x.Data.Text);

                if (!comments.Any())
                {
                    //We've found a card we haven't started tracking yet
                    _trello.Cards.AddComment(card, TrackingString);
                }
            }
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


    }
}
