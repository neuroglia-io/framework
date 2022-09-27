namespace Neuroglia.Blazor.Dagre.Models
{
    public class NodeViewModel
        : GraphElement, INodeViewModel
    {
        protected double? _x = null;
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public virtual double? X { 
            get => this._x;
            set
            {
                this._x = value;
                this.OnChange();
            }
        }

        protected double? _y = null;
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public virtual double? Y
        {
            get => this._y;
            set
            {
                this._y = value;
                this.OnChange();
            }
        }

        protected double? _width = null;
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public virtual double? Width
        {
            get => this._width;
            set
            {
                this._width = value;
                this.UpdateBBox();
            }
        }

        protected double? _height = null;
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public virtual double? Height
        {
            get => this._height;
            set
            {
                this._height = value;
                this.UpdateBBox();
            }
        }

        protected double? _radiusX = null;
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public virtual double? RadiusX
        {
            get => this._radiusX;
            set
            {
                this._radiusX = value;
                this.OnChange();
            }
        }

        protected double? _radiusY = null;
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public virtual double? RadiusY
        {
            get => this._radiusY;
            set
            {
                this._radiusY = value;
                this.OnChange();
            }
        }

        protected Guid? _parentId = null;
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public virtual Guid? ParentId
        {
            get => this._parentId;
            set
            {
                this._parentId = value;
                this.OnChange();
            }
        }

        protected string? _shape = null;
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public virtual string? Shape
        {
            get => this._shape;
            set
            {
                this._shape = value;
                this.OnChange();
            }
        }

        protected IBoundingBox _bbox = null!;
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        [Newtonsoft.Json.JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public virtual IBoundingBox? BBox => this._bbox;

        public NodeViewModel()
            : this("", null, null, Constants.NodeWidth, Constants.NodeHeight, Constants.NodeRadius, Constants.NodeRadius, 0, 0, null, null)
        { }

        public NodeViewModel(
            string? label = "",
            string? cssClass = null,
            string? shape = null,
            double? width = Constants.NodeWidth, 
            double? height = Constants.NodeHeight,
            double? radiusX = Constants.NodeRadius,
            double? radiusY = Constants.NodeRadius,
            double? x = 0,
            double? y = 0,
            Type? componentType = null,
            Guid? parentId = null
        )
            : base(label, cssClass, componentType)
        {
            this.Label = label;
            this.CssClass = cssClass;
            this.Shape = shape;
            this._width = width ?? 0;
            this._height = height ?? 0;
            this._x = x;
            this._y = y;
            this.RadiusX = radiusX ?? 0;
            this.RadiusY = radiusY ?? 0;
            this._bbox = new BoundingBox();
            this.ParentId = parentId;
            this.UpdateBBox();
        }

        public virtual void SetGeometry(double? x = null, double? y = null, double? width = null, double? height = null)
        {
            bool changed = false;
            if (x.HasValue && this._x != x)
            {
                this._x = x;
                changed = true;
            }
            if (y.HasValue && this._y != y)
            {
                this._y = y;
                changed = true;
            }
            if (width.HasValue && this._width != width)
            {
                this._width = width;
                changed = true;
            }
            if (height.HasValue && this._height != height)
            {
                this._height = height;
                changed = true;
            }
            if (changed)
            {
                this.UpdateBBox();
            }
        }

        public virtual void Move(double deltaX, double deltaY)
        {
            if (deltaX == 0 && deltaY == 0) 
                return;
            this._x += deltaX;
            this._y += deltaY;
            this.OnChange();
        }

        protected virtual void UpdateBBox()
        {
            switch (this.Shape)
            {
                case NodeShape.Circle:
                case NodeShape.Ellipse:
                    this._bbox = new BoundingBox(this.Width / 2, this.Height / 2, 0, 0);
                    break;
                default:
                    this._bbox = new BoundingBox(this.Width, this.Height, 0 - this.Width / 2, 0 - this.Height / 2);
                    break;

            }
            this.OnChange();
        }
    
    }
}
