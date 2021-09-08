"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
Object.defineProperty(exports, "__esModule", { value: true });
var core_1 = require("@angular/core");
var http_1 = require("@angular/common/http");
var litium_ui_1 = require("litium-ui");
var MediaMappingButton = /** @class */ (function (_super) {
    __extends(MediaMappingButton, _super);
    function MediaMappingButton(changeDetectorRef, http) {
        var _this = _super.call(this, changeDetectorRef) || this;
        _this.http = http;
        _this.mediaProfilers = [
            { fullName: 'Didriksons.Web.ProductMedia.Mapping.MediaProfiler', name: 'MediaProfiler' }
        ];
        _this.selectedMediaProfiler = 'Didriksons.Web.ProductMedia.Mapping.MediaProfiler';
        return _this;
    }
    MediaMappingButton.prototype.ngOnInit = function () {
        _super.prototype.ngOnInit.call(this);
    };
    MediaMappingButton.prototype.onClick = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                this.setLoading(true);
                this.http.post('/site/administration/api/mediamapping', this.getPayload(), this.getHeaders())
                    .subscribe(function () { window.location.reload(); }, function (error) { return console.error(error); });
                return [2 /*return*/];
            });
        });
    };
    MediaMappingButton.prototype.getHeaders = function () {
        return { 'headers': new http_1.HttpHeaders({ 'Content-Type': 'application/json' }) };
    };
    MediaMappingButton.prototype.getPayload = function () {
        return {
            'folderSystemId': this.getFolderSystemId(),
            'mediaProfiler': this.selectedMediaProfiler
        };
    };
    MediaMappingButton.prototype.getFolderSystemId = function () {
        return window.location.pathname.replace('/Litium/UI/media/folder/', '');
    };
    MediaMappingButton.prototype.setLoading = function (on) {
        (document.querySelectorAll('.tabpage__tab > .loading__container')[1]).style.display = on ? 'block' : 'none';
    };
    MediaMappingButton = __decorate([
        core_1.Component({
            selector: 'media-mapping-button',
            templateUrl: './media-mapping-button.component.html',
            changeDetection: core_1.ChangeDetectionStrategy.Default,
            encapsulation: core_1.ViewEncapsulation.None
        }),
        __metadata("design:paramtypes", [core_1.ChangeDetectorRef, http_1.HttpClient])
    ], MediaMappingButton);
    return MediaMappingButton;
}(litium_ui_1.BaseFieldEditor));
exports.MediaMappingButton = MediaMappingButton;
//# sourceMappingURL=media-mapping-button.component.js.map