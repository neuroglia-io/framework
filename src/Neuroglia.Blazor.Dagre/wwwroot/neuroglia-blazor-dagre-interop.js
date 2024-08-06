(() => {
    if (dagre == null) {
        throw 'dagre needs to be loaded first';
    }
    window.dagre                  = dagre;
    window.neuroglia              = window.neuroglia || {};
    window.neuroglia.blazor       = window.neuroglia.blazor || {};
    window.neuroglia.blazor.dagre = window.neuroglia.blazor.dagre || {};
    window.neuroglia.blazor.dagre.layout = (graph) => {
        dagre.layout(graph);
        return graph;
    };
    window.neuroglia.blazor.dagre.graph = (options) => {
        return new dagre.graphlib.Graph(options).setDefaultEdgeLabel(() => ({ }));
    };
    window.neuroglia.blazor.dagre.write = (graph) => {
        return JSON.stringify(dagre.graphlib.json.write(graph));
    };
    window.neuroglia.blazor.dagre.read = (str) => {
        return dagre.graphlib.json.read(JSON.parse(str));
    };
    window.neuroglia.blazor.preventScroll = (graphElement) => {
        graphElement.addEventListener("wheel", e => e.preventDefault(), { passive: false });
    }
    window.neuroglia.blazor.getCenter = (graphElement) => {
        const svgBounds = graphElement.getBoundingClientRect();
        const graphBounds = graphElement.getBBox();
        return {
            x: ((svgBounds.width - graphBounds.width) / 2) - Math.min(graphBounds.x, 0),
            y: ((svgBounds.height - graphBounds.height) / 2) - Math.min(graphBounds.y, 0)
        };
    }
    window.neuroglia.blazor.getScale = (graphElement) => {
        const svgBounds = graphElement.getBoundingClientRect();
        const graphBounds = graphElement.querySelector('g.graph').getBBox();
        const wScale = Math.floor(svgBounds.width) / Math.ceil(graphBounds.width);
        const hScale = Math.floor(svgBounds.height) / Math.ceil(graphBounds.height);
        if (graphBounds.width / graphBounds.height >= 1) {
            if (graphBounds.height * wScale < svgBounds.height) {
                return wScale;
            }
            return hScale;
        }
        else if (graphBounds.width * hScale < svgBounds.width) {
            return hScale;
        }
        return wScale;
    }
})();