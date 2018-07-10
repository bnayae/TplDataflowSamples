using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bnaya.Samples
{
    public class DataflowVisitor
    {
        public DataflowVisitor()
        {
            Items = new BindableCollection<object>();
        }
        public BindableCollection<object> Items { get; private set; }

        #region AddBlock

        public void AddBlock(BlockInformation block)
        {
            Items.Add(block);
        }

        #endregion // AddBlock

        #region RemoveBlock

        public void RemoveBlock(BlockInformation block)
        {
            Items.Remove(block);
        }

        #endregion // RemoveBlock

        #region AddLink

        public void AddLink(LinkToInformation link)
        {
            Items.Add(link);
            Items.Insert(0, link.Connector);
        }

        #endregion // AddLink

        #region RemoveLink

        public void RemoveLink(LinkToInformation link)
        {
            Items.Remove(link.Connector);
            Items.Remove(link);
        }

        #endregion // RemoveLink

        #region AddLinkCandidate

        public void AddLinkCandidate<T>(LinkCandidateInformation<T> candidate)
        {
            Items.Add(candidate);
            Items.Insert(0, candidate.Connector);
        }

        #endregion // AddLinkCandidate

        #region RemoveLinkCandidate

        public void RemoveLinkCandidate<T>(LinkCandidateInformation<T> candidate)
        {
            Items.Remove(candidate.Connector);
            Items.Remove(candidate);
        }

        #endregion // RemoveLinkCandidate

        #region Refresh

        public void Refresh()
        {
            var items = Items.ToArray();
            foreach (var item in items)
            {
                var refreshable = item as IRefreshable;
                if (refreshable != null)
                    refreshable.Refresh();
            }
        }

        #endregion // Refresh
    }
}
