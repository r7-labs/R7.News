# About R7.News

The goal of *R7.News* project is to provide a streamlined news subsystem for DNN Platform, 
which would take advantage from tight CMS integration and combinatorial approach to news article content authoring.

## Main features outline:

- [x] Mark news entry with tags and use them to provide granular thematic views.
- [x] Cast any page into news article by placing *R7.News.Agent* module onto it.
- [x] Streamline new articles creation by adding *R7.News.Agent* module into the page template.
- [ ] Keep news article page settings in sync with corresponding news entry properties.
- [ ] Export news into RSS/Atoms feeds.

### Future releases may also include:

* News sharing between portals inside the portal group.
* Content workflow (at least Draft/Publish).
* Comments and ratings (via separate module).
* JavaScript object to expose news entry properties in the skin.
* DDRMenu integration (to display recent news in the menu).

## System requirements

* DNN Platform v8.0.4.

## Install

1. Download and install [dependencies](#dependencies) first.
2. Download latest install package from releases.
3. Install it as usual from *Host &gt; Extensions*.

### <a name="dependencies">Dependencies</a>

* [R7.DotNetNuke.Extensions](https://github.com/roman-yagodin/R7.DotNetNuke.Extensions) (base library)
* [R7.ImageHandler](https://github.com/roman-yagodin/R7.ImageHandler) (automatic image thumbnailer)

## Uninstall

1. Uninstall *R7.News* library package first, this will remove all database objects and data.
2. Then uninstall *R7.News.Agent* and *R7.News.Stream* module packages (in any order).
3. Uninstall *R7.ImageHandler* and *R7.DotNetNuke.Extensions* library packages, if you don't need them.
