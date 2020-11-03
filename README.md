# About R7.News

[![BCH compliance](https://bettercodehub.com/edge/badge/roman-yagodin/R7.News)](https://bettercodehub.com/)

The goal of *R7.News* project is to provide a streamlined news subsystem for DNN Platform,
which would take advantage from tight CMS integration and combinational approach to news article content authoring.

## License

[![AGPLv3](https://www.gnu.org/graphics/agplv3-155x51.png)](https://www.gnu.org/licenses/agpl-3.0.html)

The *R7.News* is free software: you can redistribute it and/or modify it under the terms of
the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License,
or (at your option) any later version.

## Main features outline:

- [x] Mark news entry with tags and use them to provide granular thematic views.
- [x] Cast any page into news article by placing *R7.News.Agent* module onto it.
- [x] Streamline new articles creation by adding *R7.News.Agent* module into the page template.
- [x] Allow users to [discuss](#discuss) news on forum.
- [x] DDRMenu integration (display news entries in the menu).
- [x] Export news into Atom or RSS feeds.
- [x] Keep news article page settings in sync with corresponding news entry properties.

### Future releases may also include:

* News sharing between portals inside the portal group.
* Content workflow (at least Draft/Publish).

## System requirements

* DNN Platform v8.0.4.

## Install

1. Download and install [dependencies](#dependencies) first.
2. Download latest install package from releases.
3. Install it as usual from *Host &gt; Extensions*.

### <a name="dependencies">Dependencies</a>

* [R7.DotNetNuke.Extensions](https://github.com/roman-yagodin/R7.DotNetNuke.Extensions) (base library)

## <a name="discuss">Setup discussion</a>

In order to setup discussions for *R7.News*, you need do the following:

1. Install (or ensure you have installed) latest [DNN Forum](https://github.com/juvander/DotNetNuke-Forum)
   or [ActiveForums](https://github.com/ActiveForums/ActiveForums) extensions.
2. Open `R7.News.yml` config file in portal root directory in text editor.
3. Set proper values for `params` for required provider in `discuss-provider` section.
   E.g. if you have some DNN Forum module instance (moduleId=145) placed on page with tabId=40
   and you want discussion posts to be created on specific forum (forumId=2), then your configuration
   should look like this:

   ```YAML
   discuss-providers:
     - type: R7.News.Providers.DiscussProviders.DnnForumDiscussProvider
       provider-key: DnnForum
       params: ['40', '145', '2'] # tabId, moduleId, forumId
    ```

4. Comment unused providers using `#` sign.
5. Restart application to apply changes.

To disable discussions, your `discuss-providers` section in portal config file should look like this:

```YAML
discuss-providers: []
```

Note that you could develop and register your own discuss providers by implementing `IDiscussProvider` public interface.

To allow *R7.News* to use custom discuss provider:

1. Place a DLL with custom discuss provider class into `bin` folder of DNN install.
2. Register custom discuss provider using portal config file by adding assembly name:

   ```YAML
   discuss-providers:
     - type: YourCompany.DiscussProviders.YourCustomDiscussProvider, YourCompany.DiscussProviders
       provider-key: YourCustomProviderKey
       params: ['your', 'custom', 'provider', 'params', 'here']
    ```

3. Restart application to apply changes.
4. If all OK, you'll be able to create discussions for news entries using new provider.
   If not, see DNN event log for more info about what's wrong.

## DDRMenu integration

*R7.News* provides public node manipulator class for DDRMenu. In order to use it, you should do the following:

1. Add Stream module instance to the page, configure it to display required news. Node manipulator will use this module
   instance settings to get required parameters (like taxonomy terms, &quot;show all news&quot; flag, etc.)
   The page size setting value will be used as max. number of menu entries for news to display.
2. Set node manipulator options in the `R7.News.yml` config file:
   ```YAML
   node-manipulator:
     parent-node-tab-id: 77    # TabId of a parent menu node, to which news entries will be added as children
     stream-module-tab-id: 77  # TabId of a page with Stream module instance
     stream-module-id: 429     # ModuleId of Stream module instance
   ```
3. Specify `R7.News.Stream.Integrations.DDRMenu.StreamNodeManipulator` type name as NodeManipulator
   setting value in DDRMenu module settings or skinobject parameters.

Remember to check event log in case if something went wrong.

## Uninstall

1. Uninstall *R7.News* library package first, this will remove all database objects and data.
2. Uninstall *R7.News.Agent* and *R7.News.Stream* module packages (in any order).
