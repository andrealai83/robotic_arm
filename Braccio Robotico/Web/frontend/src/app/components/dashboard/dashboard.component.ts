import { Component, OnInit, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RobotService, Movimento, PositionSet } from '../../services/robot.service';

@Component({
    selector: 'app-dashboard',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
    // Connection
    ports: string[] = [];
    selectedPort: string = 'COM5';

    // Manual Control
    m1: number = 90;
    m2: number = 90;
    m3: number = 90;
    m4: number = 90;
    magnet: boolean = false;

    // Cartesian Control
    x: number = 20;
    y: number = 0;
    z: number = 10;

    // Sequences
    sequences: PositionSet[] = [];

    constructor(public robotService: RobotService) {
        effect(() => {
            // React to signal changes if needed
        });
    }

    async ngOnInit() {
        this.refreshPorts();
        this.refreshSequences();
    }

    async refreshPorts() {
        try {
            this.ports = await this.robotService.getPorts();
            if (this.ports.length > 0) this.selectedPort = this.ports[0];
        } catch (e) { console.error(e); }
    }

    async connect() {
        await this.robotService.connect(this.selectedPort);
    }

    async sendManual() {
        const mov: Movimento = {
            m1: this.m1,
            m2: this.m2,
            m3: this.m3,
            m4: this.m4,
            c: this.magnet ? 'C:1' : 'C:0'
        };
        await this.robotService.moveRaw(mov);
    }

    async sendCartesian() {
        // Use current angles as hints
        await this.robotService.moveCartesian({ x: this.x, y: this.y, z: this.z }, {
            m1: this.m1, m2: this.m2, m3: this.m3, m4: this.m4, c: this.magnet ? 'C:1' : 'C:0'
        });
        // Updating sliders to reflect new position would require return value from moveCartesian to be stored/returned
        // For now, let's assume valid move
    }

    async refreshSequences() {
        this.sequences = await this.robotService.getSequences();
    }

    async runSequence(id: number) {
        await this.robotService.runSequence(id);
    }

    // Helper to update sliders live? maybe on mouseup to avoid flooding serial
    onSliderChange() {
        // Debounce or just wait for button?
        // User asked for "Simple interface to manage movements".
        // Live control might be jerky without debouncing.
        // Let's stick to "Move" button for now, or use (change) event which fires on release.
        this.sendManual();
    }

    async home(axis: string) {
        await this.robotService.home(axis);
        // Reset sliders if successful (optimistic UI)
        if (axis === 'all') {
            this.m1 = 0; this.m2 = 0; this.m3 = 0; this.m4 = 0;
        } else if (axis === '1') this.m1 = 0;
        else if (axis === '2') this.m2 = 0;
        else if (axis === '3') this.m3 = 0;
        else if (axis === '4') this.m4 = 0;
    }
}
