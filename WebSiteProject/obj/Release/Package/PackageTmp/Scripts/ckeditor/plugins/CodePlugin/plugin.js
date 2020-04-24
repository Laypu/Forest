//import { url } from "inspector";

CKEDITOR.plugins.add('CodePlugin', {
    init: function (editor) {
        var plugName = 'CodePlugin';
        editor.addCommand(plugName, new CKEDITOR.dialogCommand(plugName));
        editor.ui.addButton('title03', {
            label: 'title03',
            icons: this.path + "/img/page_icon_01.jpg",
            click: function (editor) {
                var editorHTML = editor.innerHTML;
                var selectionStart = 0, selectionEnd = 0;
                if (editor.selectionStart) selectionStart = editor.selectionStart;
                if (editor.selectionEnd) selectionEnd = editor.selectionEnd;
                if (selectionStart !== selectionEnd) {
                    var editorCharArray = editorHTML.split("");
                    editorCharArray.splice(selectionEnd, 0, "</span>");
                    editorCharArray.splice(selectionStart, 0, "<span class='title03'>"); //must do End first
                    editorHTML = editorCharArray.join("");
                    editor.innerHTML = editorHTML;
                }

            }

        });
    }
})