var CKConfig = {
    uiColor: '#AFD6FF',
    language: 'zh',
    stylesSet: [
        { name: '大標題', element: 'h4' },
        { name: '小標題', element: 'h5' },
        { name: '段落', element: 'p' }
    ],
    contentsCss: ['../../Content/css/editor.css'],
    allowedContent: true,
    toolbar: [
        {
            name: "basicstyles",
            items: ["FontSize", "Bold", "Italic", "-", "JustifyLeft", "JustifyCenter", "JustifyRight", 'Font']
        },
        {
            name: "paragraph",
            items: ["NumberedList", "BulletedList", "-"]
        },
        {
            name: "tools",
            items: ["Maximize", "-"]
        },
        {
            name: "styles",
            items: ["Styles"]
        },
        {
            name: "links",
            items: ["Link", "Unlink", "Anchor"]
        },
        {
            name: 'insert',
            items: ['Image', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'PageBreak', 'Iframe']
        },
        {
            name: "colors",
            items: ["TextColor", "BGColor"]
        },
        { name: "editing" },
        {
            name: "document",
            items: ["Source", "-", "DocProps"]
        },
        {
            name: "clipboard",
            items: ["Cut", "Copy", "Paste", "PasteText", "PasteFromWord", "Undo", "Redo"]
        }
    ],
    filebrowserBrowseUrl: "../Scripts/ckfinder/ckfinder.html",
    filebrowserImageBrowseUrl: "../Scripts/ckfinder/ckfinder.html?Type=Images",
    filebrowserImageUploadUrl: "../Scripts/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images",
};
CKFinder.SetupCKEditor(null, "../ckfinder/");
