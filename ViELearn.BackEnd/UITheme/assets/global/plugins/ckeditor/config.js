/**
 * @license Copyright (c) 2003-2019, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here.
	// For complete reference see:
	// https://ckeditor.com/docs/ckeditor4/latest/api/CKEDITOR_config.html

	// The toolbar groups arrangement, optimized for two toolbar rows.
	config.toolbarGroups = [
		{ name: 'clipboard',   groups: [ 'clipboard', 'undo' ] },
		{ name: 'editing',     groups: [ 'find', 'selection', 'spellchecker' ] },
		{ name: 'links' },
		{ name: 'insert' },
		{ name: 'forms' },
		{ name: 'tools' },
		{ name: 'document',	   groups: [ 'mode', 'document', 'doctools' ] },
		{ name: 'others' },
		'/',
		{ name: 'basicstyles', groups: [ 'basicstyles', 'cleanup' ] },
		{ name: 'paragraph',   groups: [ 'list', 'indent', 'blocks', 'align', 'bidi' ] },
		{ name: 'styles' },
		{ name: 'colors' },
		{ name: 'about' }
	];
    config.extraPlugins = 'font,panelbutton,textwatcher,colordialog,fontawesome,justify,video,youtube,image2,toc,colorbutton';
    config.allowedContent = true;
    //config.fontAwesome_html_tag = 'i';
    //config.fontAwesome_size = 'class';
    //CKEDITOR.dtd.$removeEmpty['span'] = false;
    //CKEDITOR.dtd.$removeEmpty['i'] = false;
   
    config.contentsCss = ['/UITheme/assets/global/plugins/ckeditor/plugins/font-awesome/css/font-awesome.min.css', '/UITheme/assets/global/plugins/ckeditor/css/custom.css']; 
    //config.fontSize_sizes = '16/16px;24/24px;48/48px;';
    // Remove some buttons provided by the standard plugins, which are
    // not needed in the Standard(s) toolbar.

    // Set the most common block elements.
    config.height = 700;
    config.filebrowserBrowseUrl = '/UITheme/assets/global/plugins/ckfinder/ckfinder.html';
    config.filebrowserUploadUrl = '/UITheme/assets/global/plugins/ckfinder/core/connector/php/connector.aspx?command=QuickUpload&type=Files';
    config.filebrowserWindowWidth = '1000';
    config.filebrowserWindowHeight = '700';
	// Simplify the dialog windows.
};

