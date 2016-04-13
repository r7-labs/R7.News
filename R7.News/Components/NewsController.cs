//
//  NewsController.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016 Roman M. Yagodin
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Modules;

namespace R7.News.Components
{
    public partial class NewsController : ModuleController, IUpgradeable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="R7.News.Components.NewsController" /> class.
        /// </summary>
        public NewsController () : base ()
        {
        }

        #region IUpgradeable implementation

        string IUpgradeable.UpgradeModule (string Version)
        {
            var message = string.Format ("R7.News upgradeable actions for version {0}.)", Version);

            switch (Version) {
                case "00.01.00":
                    
                    var contentTypeController = new ContentTypeController ();

                    if (!contentTypeController.GetContentTypes ().Any (ct => ct.ContentType == "R7_News_Entry")) {
                        
                        // register new content type
                        var contentTypeId = contentTypeController.AddContentType (new ContentType ("R7_News_Entry"));
                        message += "Added content type for workflow." + Environment.NewLine;
                    
                        /*
                        var workflowActionManager = new WorkflowActionManager ();

                        // register 5 workflow actions
                        workflowActionManager.RegisterWorkflowAction (new WorkflowAction {
                            ContentTypeId = contentTypeId,
                            ActionType = WorkflowActionTypes.StartWorkflow.ToString (),
                            ActionSource = typeof (WorkflowStartAction).AssemblyQualifiedName
                        });

                        workflowActionManager.RegisterWorkflowAction (new WorkflowAction {
                            ContentTypeId = contentTypeId,
                            ActionType = WorkflowActionTypes.CompleteWorkflow.ToString (),
                            ActionSource = typeof (WorkflowCompleteAction).AssemblyQualifiedName
                        });

                        workflowActionManager.RegisterWorkflowAction (new WorkflowAction {
                            ContentTypeId = contentTypeId,
                            ActionType = WorkflowActionTypes.DiscardWorkflow.ToString (),
                            ActionSource = typeof (WorkflowDiscardAction).AssemblyQualifiedName
                        });
                        
                        workflowActionManager.RegisterWorkflowAction (new WorkflowAction {
                            ContentTypeId = contentTypeId,
                            ActionType = WorkflowActionTypes.DiscardState.ToString (),
                            ActionSource = typeof (StateDiscardAction).AssemblyQualifiedName
                        });

                        workflowActionManager.RegisterWorkflowAction (new WorkflowAction {
                            ContentTypeId = contentTypeId,
                            ActionType = WorkflowActionTypes.CompleteState.ToString (),
                            ActionSource = typeof (StateCompleteAction).AssemblyQualifiedName
                        });

                        message += "Added workflow actions. " + Environment.NewLine;

                        */
                    }
                    break;
            }

            return message;
        }

        #endregion
    }
}
