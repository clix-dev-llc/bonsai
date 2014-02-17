﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bonsai.Editor
{
    interface IWorkflowEditorService
    {
        void OnKeyDown(KeyEventArgs e);

        void OnKeyPress(KeyPressEventArgs e);

        WorkflowBuilder LoadWorkflow(string fileName);

        void OpenWorkflow(string fileName);

        void StoreWorkflowElements(WorkflowBuilder builder);

        WorkflowBuilder RetrieveWorkflowElements();

        IEnumerable<Type> GetTypeVisualizers(Type targetType);

        void StartWorkflow();

        void StopWorkflow();

        void RestartWorkflow();

        bool ValidateWorkflow();

        void RefreshEditor();

        void Undo();

        void Redo();
    }
}