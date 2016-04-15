# About R7.News

The goal of *R7.News* project is to provide a streamlined news subsystem for DNN Platform, 
which would take advantage from tight CMS integration and combinatorial approach to news article content authoring.

The project is on early stage of development, so it's not feature-complete nor stable and any changes could happen. 
ETA for main features is about the last quarter of 2016.

## Main features outline:

- [x] Mark news entry with tags and use them to provide granular thematic views.
- [x] Cast any page into news article by placing *R7.News.Agent* module onto it.
- [x] Streamline new articles creation by adding *R7.News.Agent* module into the page template.
- [ ] Keep news article page settings in sync with corresponding news entry properties.
- [ ] Export news into RSS/Atoms feeds.

## Future releases may also include:

* News sharing between portals inside the portal group.
* Content workflow (at least Draft/Publish).
* Comments and ratings (via separate module).
* JavaScript object to expose news entry properties in the skin.
* DDRMenu integration (to display recent news in the menu).

## Dependencies:

* DNN Platform 7.4.2 (DNN 8 support is planned)
* [R7.DotNetNuke.Extensions](https://github.com/roman-yagodin/R7.DotNetNuke.Extensions) (base library)
* [R7.ImageHandler](https://github.com/roman-yagodin/R7.ImageHandler) (automatic image thumbnailer)
