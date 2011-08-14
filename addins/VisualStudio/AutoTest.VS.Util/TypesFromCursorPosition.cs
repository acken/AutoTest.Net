using System;
using EnvDTE;
using EnvDTE80;
using AutoTest.Messages;

namespace AutoTest.VS.Util
{
    class Foo<T>
    {
        int x(T par)
        {
            return 0;
        }
    }

    public class OnDemandRunFromCursorPosition
    {
        private readonly DTE2 _application;

        private TextPoint _point;
        private FileCodeModel _fcm;
        private const vsCMElement _scopes = 0;

        private string _project;
        private string[] _namespace = new string[] {};
        private string[] _member = new string[] { };
        private string[] _test = new string[] { };

        public OnDemandRunFromCursorPosition(DTE2 application)
        {
            _application = application;
        }

        public OnDemandRun FromCurrentPosition()
        {
            try
            {
                getCodeFromPosition();

                foreach (vsCMElement scope in Enum.GetValues(_scopes.GetType()))
                {
                    if (!validScope(scope))
                        continue;

                    try
                    {
                        CodeElement elem = _fcm.CodeElementFromPoint(_point, scope);
                        if (elem == null)
                            continue;

                        analyzeElement(elem);
                    }
                    catch (Exception ex)
                    {
                        AutoTest.Core.DebugLog.Debug.WriteException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                AutoTest.Core.DebugLog.Debug.WriteException(ex);
            }
            if (_test.Length > 0 || _member.Length > 0)
                return new OnDemandRun(_project, _test, _member, new string[] {});
            else
                return new OnDemandRun(_project, _test, _member, _namespace);
        }

        private void analyzeElement(CodeElement elem)
        {
            _project = elem.ProjectItem.ContainingProject.FullName;
            if (elem.Kind == vsCMElement.vsCMElementNamespace)
                _namespace = new[] { elem.FullName };

            if (elem.Kind == vsCMElement.vsCMElementClass)
                _member = new[] { elem.FullName };

            if (elem.Kind == vsCMElement.vsCMElementFunction)
                _test = new[] { elem.FullName };
        }

        private void getCodeFromPosition()
        {
            var sel = (TextSelection)_application.ActiveDocument.Selection;
            _point = sel.ActivePoint;
            _fcm = _application.ActiveDocument.ProjectItem.FileCodeModel;
        }

        private static bool validScope(vsCMElement scope)
        {
            return scope == vsCMElement.vsCMElementFunction || scope == vsCMElement.vsCMElementClass || scope == vsCMElement.vsCMElementNamespace;
        }
    }
}
