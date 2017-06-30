using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Imaging.Interop;

namespace Cometary.VSIX
{
    /// <summary>
    /// Represents a source provider for a <see cref="PreviewLightBulbSource"/>.
    /// </summary>
    [Export(typeof(ISuggestedActionsSourceProvider))]
    [Name("Preview suggested actions"), ContentType("text")]
    public sealed class PreviewLightBulbSourceProvider : ISuggestedActionsSourceProvider
    {
        [Import(typeof(ITextStructureNavigatorSelectorService))]
        private ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        /// <inheritdoc />
        public ISuggestedActionsSource CreateSuggestedActionsSource(ITextView textView, ITextBuffer textBuffer)
        {
            if (textView == null || textBuffer == null)
                return null;

            return new PreviewLightBulbSource(textView, textBuffer);
        }
    }

    /// <summary>
    /// Represents a source for a <see cref="PreviewLightBulb"/>.
    /// </summary>
    public sealed class PreviewLightBulbSource : ISuggestedActionsSource
    {
        public ITextView TextView { get; }

        public ITextBuffer TextBuffer { get; }

        public PreviewLightBulbSource(ITextView textView, ITextBuffer textBuffer)
        {
            TextView = textView;
            TextBuffer = textBuffer;
        }

        /// <inheritdoc />
        public event EventHandler<EventArgs> SuggestedActionsChanged;

        /// <inheritdoc />
        public IEnumerable<SuggestedActionSet> GetSuggestedActions(
            ISuggestedActionCategorySet requestedActionCategories,
            SnapshotSpan range,
            CancellationToken cancellationToken)
        {
            if (!CanGetSuggestedActions())
                return Enumerable.Empty<SuggestedActionSet>();

            return Enumerable.Empty<SuggestedActionSet>();
        }

        /// <inheritdoc />
        public Task<bool> HasSuggestedActionsAsync(
            ISuggestedActionCategorySet requestedActionCategories,
            SnapshotSpan range,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(CanGetSuggestedActions());
        }

        private bool CanGetSuggestedActions()
        {
            return true;
        }


        /// <inheritdoc />
        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }

    /// <summary>
    /// Light bulb that shows the changed line.
    /// </summary>
    public sealed class PreviewLightBulb : ISuggestedAction
    {
        /// <inheritdoc />
        public bool HasActionSets => false;

        /// <inheritdoc />
        public bool HasPreview => true;

        /// <inheritdoc />
        public string DisplayText => "See changed syntax";

        /// <inheritdoc />
        public ImageMoniker IconMoniker => new ImageMoniker();

        /// <inheritdoc />
        public string IconAutomationText => null;

        /// <inheritdoc />
        public string InputGestureText => null;

        /// <inheritdoc />
        public Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Enumerable.Empty<SuggestedActionSet>());
        }

        /// <inheritdoc />
        public Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Invoke(CancellationToken cancellationToken)
        {
        }

        /// <inheritdoc />
        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
