import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { HttpClientModule } from '@angular/common/http';
import { UiModule } from 'litium-ui';

import { MediaMappingButton } from './components/media-mapping-button/media-mapping-button.component';

@NgModule({
    declarations: [
        MediaMappingButton
    ],
    imports: [
        CommonModule,
        UiModule,
        TranslateModule,
        HttpClientModule
    ]
})

export class MediaMapper { }