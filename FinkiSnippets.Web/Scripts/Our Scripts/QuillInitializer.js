var advancedEditor;

$(function () {
    advancedEditor = new Quill('.editor-container', {
        modules: {
            'authorship': {
                authorId: 'advanced',
                enabled: true
            },
            'toolbar': {
                container: '.toolbar-container'
            },
            'link-tooltip': true,
            'image-tooltip': true,
            'multi-cursor': true
        },
        styles: false,
        theme: 'snow'
    });
});