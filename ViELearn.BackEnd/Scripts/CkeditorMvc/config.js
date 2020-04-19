/**
 * @license Copyright (c) 2003-2017, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
    // config.uiColor = '#AADC6E';
    config.language = 'vi';
    config.uiColor = '#9AB8F3';
    ignoreEmptyParagraph = true;
    htmlEncodeOutput: true;
    //config.extraPlugins = 'notification';
    //config.extraPlugins = 'notificationaggregator';
    //config.extraPlugins = 'filetools';
    //config.extraPlugins = 'lineutils';
    //config.extraPlugins = 'widgetselection'
    //config.extraPlugins = 'widget';
    //config.extraPlugins = 'uploadwidget';
    config.extraPlugins = 'uploadimage';
};