export class MapPin {
    private _latitude: number;
    private _longitude: number;
    private _title: string;
    private _count: number;
    private _infoTitle: string;
    private _infoDesc: string;

    constructor(latitude: number, longitude: number, title: string, count: number, infoTitle: string, infoDesc: string) {
        this._infoTitle = infoTitle;
        this._infoDesc = infoDesc;
        this._latitude = latitude;
        this._longitude = longitude;
        this._title = title;
        this._count = count;
    }

    /**
     * Getter latitude
     * @return {number}
     */
    public get latitude(): number {
        return this._latitude;
    }

    /**
     * Getter longitude
     * @return { number}
     */
    public get longitude(): number {
        return this._longitude;
    }

    /**
     * Getter title
     * @return {string}
     */
    public get title(): string {
        return this._title;
    }

    /**
     * Getter count
     * @return {number}
     */
    public get count(): number {
        return this._count;
    }

    /**
     * Getter infoTitle
     * @return {string}
     */
    public get infoTitle(): string {
        return this._infoTitle;
    }

    /**
     * Getter infoDesc
     * @return {string}
     */
    public get infoDesc(): string {
        return this._infoDesc;
    }

    /**
     * Getter pinColor
     * @return {string}
     */
    public get pinColor(): string {
        if (this._count > 10)
            return 'green';
        else
            return 'red';
    }

    /**
     * Getter pinIcon
     * @return {string}
     */
    public get pinIcon(): string {
        if (this._count > 10)
            return 'assets/icons/drop_pin_green.png';
        else
            return 'assets/icons/drop_pin_red.png';
    }

    /**
     * Setter latitude
     * @param {number} value
     */
    public set latitude(value: number) {
        this._latitude = value;
    }

    /**
     * Setter longitude
     * @param { number} value
     */
    public set longitude(value: number) {
        this._longitude = value;
    }

    /**
     * Setter title
     * @param {string} value
     */
    public set title(value: string) {
        this._title = value;
    }

    /**
     * Setter count
     * @param {number} value
     */
    public set count(value: number) {
        this._count = value;
    }

    /**
     * Setter infoTitle
     * @param {string} value
     */
    public set infoTitle(value: string) {
        this._infoTitle = value;
    }

    /**
     * Setter infoDesc
     * @param {string} value
     */
    public set infoDesc(value: string) {
        this._infoDesc = value;
    }


}