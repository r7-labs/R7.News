# About R7.News

The goal of *R7.News* project is to provide a streamlined news subsystem for DNN Platform, 
which would take advantage from tight CMS integration and combinatorial approach to news article content authoring.

The project is on early stage of development, so it's not feature-complete nor stable and any changes could happen. 
ETA for main features is about the last quarter of 2016.

## Main features outline:

* Mark news entry with tags and use them to provide granular thematic views.
* Cast any page into news article by placing news agent module.
* Streamline new articles creation by adding news agent module to the page template.
* Keep news article page settings in sync with corresponding news entry properties.
* Export news into RSS/Atoms feeds.

## Future releases could also include:

* News sharing between portals in the same portal group.
* JavaScript object to access news entry properties in the skin.
* Content workflow (at least Draft/Publish).
* Comments and ratings (via separate module).
* DDRMenu integration (to display recent news in the menu).

# Dependencies:

* DNN Platform 7.4.2 (DNN 8 support is planned)
* [DotNetNuke.R7](https://github.com/roman-yagodin/DotNetNuke.R7) (base library)
* [R7.ImageHandler](https://github.com/roman-yagodin/R7.ImageHandler) (automatic thumbnailer)
