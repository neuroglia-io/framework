(() => {
    if (dagre == null) {
        throw 'dagre needs to be loaded first';
    }

    function copyStyle(element) {
        const style = getComputedStyle(element);
        Object.entries(style).forEach(([key, value]) => {
            const styleName = key.replace(/\-([a-z])/g, match => match[1].toUpperCase());
            element.style[styleName] = value;
        });
        Array.from(element.childNodes)
            .filter(node => node.nodeType == Node.ELEMENT_NODE)
            .forEach(node => copyStyle(node));
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
            x: ((svgBounds.width - graphBounds.width) / 2),
            y: ((svgBounds.height - graphBounds.height) / 2)
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
    window.neuroglia.blazor.saveAsPng = (graphElement) => {
        const canvas = document.createElement('canvas');
        const context = canvas.getContext('2d');
        const boundingBox = graphElement.querySelector('g.graph').getBBox();
        canvas.setAttribute('width', boundingBox.width + 'px');
        canvas.setAttribute('height', boundingBox.height + 'px');
        canvas.setAttribute('style', 'display: none;');
        const img = new Image(boundingBox.width, boundingBox.height);
        img.onerror = (err) => {
            console.error('There was an error loading the SVG as base64.');
        };
        img.onload = async () => {
            context.drawImage(img, 0, 0, img.width, img.height);
            const downloadLink = document.createElement('a');
            downloadLink.download = 'diagram.png';
            downloadLink.href = canvas.toDataURL('image/png', 1);
            downloadLink.click();
            document.body.removeChild(canvas);
            document.body.removeChild(img);
            document.body.removeChild(svgClone);
        };
        const svgClone = graphElement.cloneNode(true);
        const defsEl = svgClone.querySelector('defs');
        const graphElClone = svgClone.querySelector('g.graph');
        graphElClone.setAttribute('transform', 'scale(1)');
        document.body.appendChild(svgClone);
        Array.from(document.querySelectorAll('.svg-definitions defs *')).forEach(def => {
            defsEl.appendChild(def.cloneNode(true));
        });
        copyStyle(svgClone);
        const svg = new XMLSerializer().serializeToString(svgClone);
        const base64 = btoa(unescape(encodeURIComponent(svg)));
        document.body.appendChild(canvas);
        document.body.appendChild(img);
        img.src = `data:image/svg+xml;charset=utf-8;base64,${base64}`;
    }
})();