using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using SilAPI;
using SilUI;

namespace Sil.VS2012
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget, IDisassemblyProvider
	{
        /// <summary>
        /// Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="connectMode">The connect mode.</param>
        /// <param name="addInInst">The add in inst.</param>
        /// <param name="custom">The custom.</param>
        /// <seealso class="IDTExtensibility2" />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			_applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;

            try
            {
                switch (connectMode)
                {
                    case ext_ConnectMode.ext_cm_UISetup:

                        //  Create commands in the UI Setup phase. 
                        //  This phase is called only once when the add-in is deployed.
                        CreateCommands();
                        
                        break;

                    case ext_ConnectMode.ext_cm_AfterStartup:

                        //  After startup is complete, initialise the addin.
                        InitializeAddIn();

                        break;

                    case ext_ConnectMode.ext_cm_Startup:

                        // Do nothing yet, wait until the IDE is fully initialized (OnStartupComplete will be called)
                        break;
                }
            }
            catch (Exception ex)
            {
                //  Handle the exception.
                HandleException(@"Failed to initialise Sil.", ex);
            }
        }

        private void InitializeAddIn()
        {
            //  Get the sil command name.
            var silCommandId = _addInInstance.ProgID + "." + Command_SeeIL_CommandName;

            //  If we don't have the command, create it.
            if (_applicationObject.Commands.OfType<Command>().Any(c => c.Name != silCommandId))
                CreateCommands();

            //  Create the control for the Sil command.
            try
            {
                var commandSil = _applicationObject.Commands.Item(_addInInstance.ProgID + "." + Command_SeeIL_CommandName);
                
                //  Retrieve the context menu of code windows.
                var commandBars = (CommandBars)_applicationObject.CommandBars;
                var codeWindowCommandBar = commandBars["Code Window"];

                //  Add the one line command.
                commandSil.AddControl(codeWindowCommandBar,
                   codeWindowCommandBar.Controls.Count + 1);
            }
            catch (Exception exception)
            {
                HandleException(@"Failed to create the 'Disassemble' command.", exception);
            }
        }

        /// <summary>
        /// Creates the commands.
        /// </summary>
        private void CreateCommands()
        {
            try
            {

                var contextUIGuids = new object[] { };

                //  Create the 'See IL' command.
                _applicationObject.Commands.AddNamedCommand(_addInInstance, Command_SeeIL_CommandName,
                    Command_SeeIL_Caption, Command_SeeIL_Tooltip, false, 1,
                   ref contextUIGuids, (int)vsCommandStatus.vsCommandStatusSupported);
            }
            catch (Exception)
            {
                //  Creating the command should only fail if we have it already.
            }
        }

        /// <summary>
        /// Handles exceptions.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        private void HandleException(string message, Exception exception)
        {
            //  Currently, for an exception we just show the message box.
            MessageBox.Show(message + Environment.NewLine +
                exception, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.
        /// </summary>
        /// <param name="disconnectMode">The disconnect mode.</param>
        /// <param name="custom">The custom.</param>
        /// <seealso class="IDTExtensibility2" />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
		}

        /// <summary>
        /// Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.
        /// </summary>
        /// <param name="custom">The custom.</param>
        /// <seealso class="IDTExtensibility2" />
		public void OnAddInsUpdate(ref Array custom)
		{
		}

        /// <summary>
        /// Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.
        /// </summary>
        /// <param name="custom">The custom.</param>
        /// <seealso class="IDTExtensibility2" />
		public void OnStartupComplete(ref Array custom)
        {
            InitializeAddIn();
		}

        /// <summary>
        /// Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.
        /// </summary>
        /// <param name="custom">The custom.</param>
        /// <seealso class="IDTExtensibility2" />
		public void OnBeginShutdown(ref Array custom)
		{
		}
		
		private DTE2 _applicationObject;
		private AddIn _addInInstance;

	    private const string Command_SeeIL_CommandName = @"SeeIntermediateLanguage";
	    private const string Command_SeeIL_Caption = @"Disassemble";
	    private const string Command_SeeIL_Tooltip = @"Disassemble the selection";


        /// <summary>
        /// Queries the status.
        /// </summary>
        /// <param name="CmdName">Name of the CMD.</param>
        /// <param name="NeededText">The needed text.</param>
        /// <param name="StatusOption">The status option.</param>
        /// <param name="CommandText">The command text.</param>
	    public void QueryStatus(string CmdName, vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
	    {
            if (CmdName == _addInInstance.ProgID + "." + Command_SeeIL_CommandName)
            {
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
                StatusOption = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
// ReSharper restore BitwiseOperatorOnEnumWithoutFlags
            }
	    }

        /// <summary>
        /// Execs the specified CMD name.
        /// </summary>
        /// <param name="CmdName">Name of the CMD.</param>
        /// <param name="ExecuteOption">The execute option.</param>
        /// <param name="VariantIn">The variant in.</param>
        /// <param name="VariantOut">The variant out.</param>
        /// <param name="Handled">if set to <c>true</c> [handled].</param>
	    public void Exec(string CmdName, vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            if (CmdName == _addInInstance.ProgID + "." + Command_SeeIL_CommandName)
            {
                try
                {
                    ExecuteSilCommand();
                }
                catch (Exception exception)
                {
                    HandleException("There was a problem performing the disassembly.", exception);
                }
            }
	    }

        private void ExecuteSilCommand()
        {
            //  Create a new GUID for the window.
            var guid = Guid.NewGuid().ToString();

            //  Create a new object to hold the control object.
            var controlObject = new object();

            //  Get the application windows.
            var windows = (Windows2)_applicationObject.Windows;

            //  Get the type of the Sil View Host.
            var silViewHostType = typeof (SilViewHostWrapper);

            //  Get the view host assembly and class name.
            var hostAssemblyPath = silViewHostType.Assembly.Location;
            var hostClassName = silViewHostType.FullName;

            //  Identify the disassembly targets.
            var targets = IdentifyDisassemblyTargets().ToList();
            
            //  Create the window.
            var window = (Window2)windows.CreateToolWindow2(_addInInstance, hostAssemblyPath,
                                                             hostClassName, "Sil", guid, ref controlObject);
            
            //  Get the sil host.
            var host = (SilViewHostWrapper)controlObject;

            //  Initialise the host with the parent window.
            host.InitialiseParentWindow(window);
            
            //  Initialise the host.
            host.SilViewHost.InitialiseView(this, targets.FirstOrDefault());

            //  Ensure that the window is visible.
            window.Visible = true;
        }

        private IEnumerable<DisassemblyTarget> IdentifyDisassemblyTargets()
        {
            //  Get the active point.
            var textSelection = (TextSelection)_applicationObject.ActiveDocument.Selection;
            var activePoint = textSelection.ActivePoint;
            
            //  We'll check for these targets in order.
            var element = activePoint.CodeElement[vsCMElement.vsCMElementVariable];
            if (element != null)
                yield return new DisassemblyTarget(DisassemblyTargetType.Field, element.FullName);

            element = activePoint.CodeElement[vsCMElement.vsCMElementEvent];
            if (element != null)
                yield return new DisassemblyTarget(DisassemblyTargetType.Event, element.FullName);

            element = activePoint.CodeElement[vsCMElement.vsCMElementDelegate];
            if (element != null)
                yield return new DisassemblyTarget(DisassemblyTargetType.Delegate, element.FullName);

            element = activePoint.CodeElement[vsCMElement.vsCMElementFunction];
            if (element != null)
                yield return new DisassemblyTarget(DisassemblyTargetType.Method, element.FullName);

            element = activePoint.CodeElement[vsCMElement.vsCMElementProperty];
            if (element != null)
                yield return new DisassemblyTarget(DisassemblyTargetType.Property, element.FullName);

            element = activePoint.CodeElement[vsCMElement.vsCMElementClass];
            if (element != null)
                yield return new DisassemblyTarget(DisassemblyTargetType.Class, element.FullName);

            element = activePoint.CodeElement[vsCMElement.vsCMElementInterface];
            if (element != null)
                yield return new DisassemblyTarget(DisassemblyTargetType.Interface, element.FullName);

            element = activePoint.CodeElement[vsCMElement.vsCMElementStruct];
            if (element != null)
                yield return new DisassemblyTarget(DisassemblyTargetType.Structure, element.FullName);
        }

	    Task<DisassembledAssembly> IDisassemblyProvider.DisassembleAssembly()
	    {
            //  We need the project.
            if (_applicationObject.ActiveDocument == null ||
                _applicationObject.ActiveDocument.ProjectItem == null ||
                _applicationObject.ActiveDocument.ProjectItem.ContainingProject == null)
                throw new InvalidOperationException("There is no current project to disassemble.");

	        var activeProject = _applicationObject.ActiveDocument.ProjectItem.ContainingProject;

            //  From the active project, get the output path.
            var activeConfig = activeProject.ConfigurationManager.ActiveConfiguration;

            //  Get the path.
            var FullPath = activeProject.Properties.Item("FullPath").Value.ToString();
            var InputAssembly = activeConfig.Properties.Item("CodeAnalysisInputAssembly").Value.ToString();
	        var assemblyPath = Path.Combine(FullPath, InputAssembly);

            return Task<DisassembledAssembly>.Factory.StartNew(() => Disassembler.DisassembleAssembly(assemblyPath));
	    }
	}
}