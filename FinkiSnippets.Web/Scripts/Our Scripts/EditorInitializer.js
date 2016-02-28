$(function () {
    var editors = $(".codeEditor");
    
    editors.each(function (idx) {

        var editor = ace.edit("editor_" + idx);
        editor.setTheme("ace/theme/eclipse");
        editor.getSession().setMode("ace/mode/java");
        editor.setOptions({
            readOnly: true,
            highlightActiveLine: false,
            highlightGutterLine: false
        });

        editor.renderer.$cursorLayer.element.style.opacity = 0;

        var heightUpdateFunction = function () {

            // http://stackoverflow.com/questions/11584061/
            var newHeight =
                      editor.getSession().getScreenLength()
                      * editor.renderer.lineHeight
                      + editor.renderer.scrollBar.getWidth();

            $('#editor_' + idx).height(newHeight.toString() + "px");
            $('#editor-section').height(newHeight.toString() + "px");

            // This call is required for the editor to fix all of
            // its inner structure for adapting to a change in size
            editor.resize();
        };

        heightUpdateFunction();
        editor.getSession().on('change', heightUpdateFunction);

        Variables.initEditors[idx] = editor;
    });
});

function InitializeEditors()
{
    var editors = $(".codeEditor");

    editors.each(function (idx) {

        var editor = ace.edit("editor_" + idx);
        editor.setTheme("ace/theme/eclipse");
        editor.getSession().setMode("ace/mode/java");
        editor.setOptions({
            readOnly: true,
            highlightActiveLine: false,
            highlightGutterLine: false
        });

        editor.renderer.$cursorLayer.element.style.opacity = 0;

        var heightUpdateFunction = function () {

            // http://stackoverflow.com/questions/11584061/
            var newHeight =
                      editor.getSession().getScreenLength()
                      * editor.renderer.lineHeight
                      + editor.renderer.scrollBar.getWidth();

            $('#editor_' + idx).height(newHeight.toString() + "px");
            $('#editor-section').height(newHeight.toString() + "px");

            // This call is required for the editor to fix all of
            // its inner structure for adapting to a change in size
            editor.resize();
        };

        heightUpdateFunction();
        editor.getSession().on('change', heightUpdateFunction);

        Variables.initEditors[idx] = editor;
    });
}