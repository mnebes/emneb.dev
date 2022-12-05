#r "../_lib/Fornax.Core.dll"
#if !FORNAX
#load "../loaders/postloader.fsx"
#load "../loaders/pageloader.fsx"
#load "../loaders/globalloader.fsx"
#endif

open Html

let injectWebsocketCode (webpage:string) =
    let websocketScript =
        """
        <script type="text/javascript">
          var wsUri = "ws://localhost:8080/websocket";
      function init()
      {
        websocket = new WebSocket(wsUri);
        websocket.onclose = function(evt) { onClose(evt) };
      }
      function onClose(evt)
      {
        console.log('closing');
        websocket.close();
        document.location.reload();
      }
      window.addEventListener("load", init, false);
      </script>
        """
    let head = "<head>"
    let index = webpage.IndexOf head
    webpage.Insert ( (index + head.Length + 1),websocketScript)

let layout (ctx : SiteContents) active bodyCnt =
    let pages = ctx.TryGetValues<Pageloader.Page> () |> Option.defaultValue Seq.empty
    let siteInfo = ctx.TryGetValue<Globalloader.SiteInfo> ()
    let ttl =
        siteInfo
        |> Option.map (fun si -> si.title)
        |> Option.defaultValue ""

    let menuEntries =
        pages
        |> Seq.map (fun p ->
            let cls = if p.title = active then "navbar-item is-active" else "navbar-item"
            a [Class cls; Href p.link] [!! p.title ])
        |> Seq.toList

    html [] [
        head [] [
            meta [CharSet "utf-8"]
            meta [Name "viewport"; Content "width=device-width, initial-scale=1"]
            title [] [!! ttl]
            link [Rel "icon"; Type "image/png"; Sizes "32x32"; Href "/images/favicon.png"]
            link [Rel "stylesheet"; Href "https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css"]
            link [Rel "stylesheet"; Href "https://fonts.googleapis.com/css?family=Open+Sans"]
            link [Rel "stylesheet"; Href "https://cdn.jsdelivr.net/npm/bulma@0.9.4/css/bulma.min.css"]
            // link [Rel "stylesheet"; Href "https://unpkg.com/bulmaswatch/minty/bulmaswatch.min.css"]
            link [Rel "stylesheet"; Type "text/css"; Href "/style/style.css"]
            link [Rel "stylesheet"; Href "https://unpkg.com/@highlightjs/cdn-assets@11.6.0/styles/monokai.min.css"]
            script [Src "//unpkg.com/@highlightjs/cdn-assets@11.6.0/highlight.min.js" ] [ ]
            script [Src "//unpkg.com/@highlightjs/cdn-assets@11.6.0/languages/fsharp.min.js" ] [ ]
            script [] [ !! "hljs.highlightAll();" ]
        ]
        body [] [
            nav [Class "navbar"] [
                div [Class "container"] [
                    span [Class "navbar-burger burger"; HtmlProperties.Custom("data-target", "navbarMenu")] [
                        span [] []
                        span [] []
                        span [] []
                    ]
                    div [Id "navbarMenu"; Class "navbar-menu"] menuEntries
                ]
            ]
            yield! bodyCnt
            script [Src "/js/nav.js"] []
        ]
    ]

let render (ctx : SiteContents) cnt =
    let disableLiveRefresh = ctx.TryGetValue<Postloader.PostConfig> () |> Option.map (fun n -> n.disableLiveRefresh) |> Option.defaultValue false
    cnt
    |> HtmlElement.ToString
    |> fun n -> if disableLiveRefresh then n else injectWebsocketCode n

let published (post: Postloader.Post) =
    post.published
    |> Option.defaultValue System.DateTime.Now
    |> fun n -> n.ToString("yyyy-MM-dd")

let postLayout (useSummary: bool) (post: Postloader.Post) =
    article [Class "box article"] [

        img [Class "author-image"; Src "/images/author.png"]
        div [Class "block has-text-centered"] [
            p [Class "title article-title"; ] [ a [Href post.link] [!! post.title]]
            match post.subtitle with
            | Some subtitle -> p [Class "subtitle is-8 article-subtitle"] [ i [] [ !! subtitle ] ]
            | None -> !!""
            p [Class "subtitle is-6 article-subtitle"] [
                a [Href "/about.html"] [!! (defaultArg post.author "")]
                !! (sprintf "on %s" (published post))
            ]
            for tag in post.tags do
              span [Class "tag is-primary"] [ !!tag ]
        ]
        div [Class "content article-body"] [
            !! (if useSummary then post.summary else post.content)
        ]
        div [Class "block"] []
        if useSummary then
            nav [Class "level"] [
                div [Class "level-left"] []
                div [Class "level-right"] [
                    p [Class "level-item is-primary"] [ a [Href post.link] [!! "read more"] ]
                ]
            ]

    ]
