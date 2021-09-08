import { Component, ChangeDetectorRef, ChangeDetectionStrategy, OnInit, ViewEncapsulation } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseFieldEditor } from 'litium-ui';

@Component({
    selector: 'media-mapping-button',
    templateUrl: './media-mapping-button.component.html',
    changeDetection: ChangeDetectionStrategy.Default,
    encapsulation: ViewEncapsulation.None
})
export class MediaMappingButton extends BaseFieldEditor implements OnInit {
    constructor(changeDetectorRef: ChangeDetectorRef, private http: HttpClient) {
        super(changeDetectorRef);
    }

    ngOnInit() {
        super.ngOnInit();
    }

    async onClick() {
        this.setLoading(true);

        this.http.post('/site/administration/api/mediamapping', this.getPayload(), this.getHeaders())
            .subscribe(() => { window.location.reload() }, error => console.error(error));
    }

    getHeaders(): Object {
        return { 'headers': new HttpHeaders({ 'Content-Type': 'application/json' }) };
    }

    getPayload(): Object {
        return {
            'folderSystemId': this.getFolderSystemId()
        };
    }

    getFolderSystemId(): string {
        return window.location.pathname.replace('/Litium/UI/media/folder/', '');
    }

    setLoading(on: boolean): void {
        (<HTMLElement>(document.querySelectorAll('.tabpage__tab > .loading__container')[1])).style.display = on ? 'block' : 'none';
    }
}